using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;
using SupplyChainSystem.Server.ResponseWrapper;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AgreementController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public AgreementController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        public SupplyResponse Get()
        {
            var agreements = _dbContext.Agreement.Select(p => p).Include(p => p.BlanketPurchaseAgreementDetails)
                .Include(p => p.BlanketPurchaseAgreementLines).ThenInclude(p => p.Item)
                .Include(p => p.ContractPurchaseAgreementDetails).Include(p => p.ContractPurchaseAgreementLines)
                .ThenInclude(p => p.Item).Include(p => p.PlannedPurchaseAgreementDetails)
                .Include(p => p.PlannedPurchaseAgreementLines).ThenInclude(p => p.Item);
            return SupplyResponse.Ok(agreements);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var agreement = _dbContext.Agreement.Include(p => p.BlanketPurchaseAgreementDetails)
                .Include(p => p.BlanketPurchaseAgreementLines).ThenInclude(p => p.Item)
                .Include(p => p.ContractPurchaseAgreementDetails).Include(p => p.ContractPurchaseAgreementLines)
                .ThenInclude(p => p.Item).Include(p => p.PlannedPurchaseAgreementDetails)
                .Include(p => p.PlannedPurchaseAgreementLines).ThenInclude(p => p.Item)
                .SingleOrDefault(p => p.AgreementId == id);
            return SupplyResponse.Ok(agreement);
        }

        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] AgreementWrapper agreement)
        {
            var currentUser = HttpContext.User;
            var dbUser =
                _dbContext.User.Include(p => p.RestaurantManager).ThenInclude(p => p.Restaurant)
                    .SingleOrDefault(p => currentUser.FindFirst(ClaimTypes.Name).Value.Equals(p.UserName));
            if (dbUser == null) return SupplyResponse.Fail("Unauthorize", "Your are not the user in the system.");

            if (agreement.Details == null ||
                agreement.Items == null || agreement.SupplierId == 0)
            {
                return SupplyResponse.RequiredFieldEmpty();
            }

            if (agreement.StartDate > agreement.ExpiryDate)
            {
                return SupplyResponse.BadRequest("Start date cannot be later than Expiry Date");
            }


            //BPA
            if (agreement.AgreementType == AgreementType.Blanket)
            {
                //Get data from request
                ICollection<QuantityItems> items = new List<QuantityItems>();
                BlanketPurchaseAgreementDetails details;
                try
                {
                    foreach (var item in agreement.Items)
                    {
                        items.Add(item.ToObject<QuantityItems>());
                    }

                    details = agreement.Details.ToObject<BlanketPurchaseAgreementDetails>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return SupplyResponse.BadRequest("Request Format Fail");
                }


                //Verfiy&Process request object
                var dbLine = new Dictionary<string, BlanketPurchaseAgreementLine>();

                foreach (var item in items)
                {
                    if (item.MinimumQuantity <= 0 && item.PromisedQuantity <= 0)
                    {
                        return SupplyResponse.BadRequest($"Item {item.SupplierItemId} has a zero or negative quantity");
                    }

                    var dbItem = _dbContext.Item.SingleOrDefault(p => item.SupplierItemId == p.SupplierItemId);
                    if (dbItem == null)
                    {
                        return SupplyResponse.NotFound("supplier item", item.SupplierItemId);
                    }

                    if (dbLine.ContainsKey(item.SupplierItemId))
                    {
                        return SupplyResponse.DuplicateEntry("Request Item", item.SupplierItemId);
                    }

                    dbLine[item.SupplierItemId] = new BlanketPurchaseAgreementLine
                    {
                        ItemId = dbItem.Id,
                        MinimumQuantity = item.MinimumQuantity,
                        PromisedQuantity = item.PromisedQuantity,
                        Price = item.Price,
                        Unit = item.Unit,
                    };
                }

                //Create Agreement Object

                var dbAgreement = new Agreement
                {
                    AgreementType = AgreementType.Blanket,
                    Currency = agreement.Currency,
                    StartDate = agreement.StartDate,
                    ExpiryDate = agreement.ExpiryDate,
                    SupplierId = agreement.SupplierId,
                    CreateBy = dbUser.UserId,
                    TermsAndCondition = agreement.TermsAndCondition
                };
                _dbContext.Agreement.Add(dbAgreement);
                _dbContext.SaveChanges();
                var agreementId = dbAgreement.AgreementId;
                _dbContext.Entry(dbAgreement).State = EntityState.Detached;

                details.AgreementId = agreementId;
                _dbContext.BlanketPurchaseAgreementDetails.Add(details);
                _dbContext.SaveChanges();

                foreach (var line in dbLine.Values)
                {
                    line.AgreementId = agreementId;
                    var entry = _dbContext.BlanketPurchaseAgreementLine.Add(line);
                    _dbContext.SaveChanges();
                    entry.State = EntityState.Detached;
                }

                return Get(agreementId);
            }
            else if (agreement.AgreementType == AgreementType.Contract) //CPA
            {
                ICollection<QuantityItems> items = new List<QuantityItems>();
                ContractPurchaseAgreementDetails details;
                try
                {
                    foreach (var item in agreement.Items)
                    {
                        items.Add(item.ToObject<QuantityItems>());
                    }

                    details = agreement.Details.ToObject<ContractPurchaseAgreementDetails>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return SupplyResponse.BadRequest("Request Format Fail");
                }

                var dbLine = new List<ContractPurchaseAgreementLine>();

                foreach (var item in items)
                {
                    var dbItem = _dbContext.Item.SingleOrDefault(p => item.SupplierItemId == p.SupplierItemId);
                    if (dbItem == null)
                    {
                        return SupplyResponse.NotFound("supplier item", item.SupplierItemId);
                    }

                    dbLine.Add(new ContractPurchaseAgreementLine()
                    {
                        ItemId = dbItem.Id,
                    });
                }

                var dbAgreement = new Agreement
                {
                    AgreementType = AgreementType.Contract,
                    Currency = agreement.Currency,
                    StartDate = agreement.StartDate,
                    ExpiryDate = agreement.ExpiryDate,
                    SupplierId = agreement.SupplierId,
                    CreateBy = dbUser.UserId,
                    TermsAndCondition = agreement.TermsAndCondition
                };

                _dbContext.Agreement.Add(dbAgreement);
                _dbContext.SaveChanges();
                var agreementId = dbAgreement.AgreementId;
                _dbContext.Entry(dbAgreement).State = EntityState.Detached;

                details.AgreementId = agreementId;
                _dbContext.ContractPurchaseAgreementDetails.Add(details);
                _dbContext.SaveChanges();

                foreach (var line in dbLine)
                {
                    line.AgreementId = agreementId;
                    var entry = _dbContext.ContractPurchaseAgreementLine.Add(line);
                    _dbContext.SaveChanges();
                    entry.State = EntityState.Detached;
                }

                return Get(agreementId);
            }
            else if (agreement.AgreementType == AgreementType.Planned)
            {
                ICollection<QuantityItems> items = new List<QuantityItems>();
                PlannedPurchaseAgreementDetails details;
                try
                {
                    foreach (var item in agreement.Items)
                    {
                        items.Add(item.ToObject<QuantityItems>());
                    }

                    details = agreement.Details.ToObject<PlannedPurchaseAgreementDetails>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return SupplyResponse.BadRequest("Request Format Fail");
                }

                var dbLine = new Dictionary<string, PlannedPurchaseAgreementLine>();

                foreach (var item in items)
                {
                    if (item.Quantity <= 0)
                    {
                        return SupplyResponse.BadRequest($"Item {item.SupplierItemId} has a zero or negative quantity");
                    }

                    var dbItem = _dbContext.Item.SingleOrDefault(p => item.SupplierItemId == p.SupplierItemId);
                    if (dbItem == null)
                    {
                        return SupplyResponse.NotFound("supplier item", item.SupplierItemId);
                    }

                    if (dbLine.ContainsKey(item.SupplierItemId))
                    {
                        return SupplyResponse.DuplicateEntry("Request Item", item.SupplierItemId);
                    }

                    dbLine[item.SupplierItemId] = new PlannedPurchaseAgreementLine
                    {
                        ItemId = dbItem.Id,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Unit = item.Unit
                    };
                }

                var dbAgreement = new Agreement
                {
                    AgreementType = AgreementType.Planned,
                    Currency = agreement.Currency,
                    StartDate = agreement.StartDate,
                    ExpiryDate = agreement.ExpiryDate,
                    SupplierId = agreement.SupplierId,
                    CreateBy = dbUser.UserId,
                    TermsAndCondition = agreement.TermsAndCondition
                };
                _dbContext.Agreement.Add(dbAgreement);
                _dbContext.SaveChanges();
                var agreementId = dbAgreement.AgreementId;
                _dbContext.Entry(dbAgreement).State = EntityState.Detached;

                details.AgreementId = agreementId;
                _dbContext.PlannedPurchaseAgreementDetails.Add(details);
                _dbContext.SaveChanges();

                foreach (var line in dbLine.Values)
                {
                    line.AgreementId = agreementId;
                    var entry = _dbContext.PlannedPurchaseAgreementLine.Add(line);
                    _dbContext.SaveChanges();
                    entry.State = EntityState.Detached;
                }

                return Get(agreementId);
            }

            return SupplyResponse.NotFound("Agreement Type", agreement.AgreementType + "");
        }


        //+==================PUT
        //+==================PUT
        //+==================PUT
        //+==================PUT
        //+==================PUT

        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] AgreementWrapper agreement)
        {
            var currentUser = HttpContext.User;
            var dbUser =
                _dbContext.User.Include(p => p.RestaurantManager).ThenInclude(p => p.Restaurant)
                    .SingleOrDefault(p => currentUser.FindFirst(ClaimTypes.Name).Value.Equals(p.UserName));
            if (dbUser == null) return SupplyResponse.Fail("Unauthorize", "Your are not the user in the system.");

            if (agreement.Details == null ||
                agreement.Items == null || agreement.SupplierId == 0)
            {
                return SupplyResponse.RequiredFieldEmpty();
            }

            if (agreement.StartDate > agreement.ExpiryDate)
            {
                return SupplyResponse.BadRequest("Start date cannot be later than Expiry Date");
            }

            var _dbAgreement = _dbContext.Agreement.AsNoTracking().SingleOrDefault(p => id == p.AgreementId);

            if (_dbAgreement == null) return SupplyResponse.NotFound("Agreement", id + "");
            if (_dbAgreement.AgreementType != agreement.AgreementType)
                return SupplyResponse.BadRequest("Agreement Type Cannot be change");


            //BPA
            if (agreement.AgreementType == AgreementType.Blanket)
            {
                ICollection<QuantityItems> items = new List<QuantityItems>();
                BlanketPurchaseAgreementDetails details;
                try
                {
                    foreach (var item in agreement.Items)
                    {
                        items.Add(item.ToObject<QuantityItems>());
                    }

                    details = agreement.Details.ToObject<BlanketPurchaseAgreementDetails>();
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return SupplyResponse.BadRequest("Request Format Fail");
                }

                var dbLine = new Dictionary<string, BlanketPurchaseAgreementLine>();

                foreach (var item in items)
                {
                    if (item.MinimumQuantity <= 0 && item.PromisedQuantity <= 0)
                    {
                        return SupplyResponse.BadRequest($"Item {item.SupplierItemId} has a zero or negative quantity");
                    }

                    var dbItem = _dbContext.Item.SingleOrDefault(p => item.SupplierItemId == p.SupplierItemId);
                    if (dbItem == null)
                    {
                        return SupplyResponse.NotFound("supplier item", item.SupplierItemId);
                    }

                    if (dbLine.ContainsKey(item.SupplierItemId))
                    {
                        return SupplyResponse.DuplicateEntry("Request Item", item.SupplierItemId);
                    }

                    dbLine[item.SupplierItemId] = new BlanketPurchaseAgreementLine
                    {
                        ItemId = dbItem.Id,
                        MinimumQuantity = item.MinimumQuantity,
                        PromisedQuantity = item.PromisedQuantity,
                        Price = item.Price,
                        Unit = item.Unit,
                    };
                }

                var _dbDetails = _dbContext.BlanketPurchaseAgreementDetails.AsNoTracking()
                    .SingleOrDefault(p => p.AgreementId == _dbAgreement.AgreementId);
                _dbContext.Remove(_dbDetails);

                var lines = _dbContext.BlanketPurchaseAgreementLine.Select(p => p)
                    .Where(p => p.AgreementId == _dbAgreement.AgreementId);

                foreach (var line in lines)
                {
                    _dbContext.Remove(line);
                }

                _dbContext.SaveChanges();

                var dbAgreement = new Agreement
                {
                    AgreementId = _dbAgreement.AgreementId,
                    AgreementType = AgreementType.Blanket,
                    Currency = agreement.Currency,
                    StartDate = agreement.StartDate,
                    ExpiryDate = agreement.ExpiryDate,
                    SupplierId = agreement.SupplierId,
                    CreateBy = _dbAgreement.CreateBy,
                    TermsAndCondition = agreement.TermsAndCondition
                };
                var agreementEntry = _dbContext.Agreement.Attach(dbAgreement);
                agreementEntry.State = EntityState.Modified;
                _dbContext.SaveChanges();
                _dbContext.Entry(dbAgreement).State = EntityState.Detached;


                details.AgreementId = _dbAgreement.AgreementId;
                _dbContext.BlanketPurchaseAgreementDetails.Add(details);
                _dbContext.SaveChanges();


                foreach (var line in dbLine.Values)
                {
                    line.AgreementId = _dbAgreement.AgreementId;
                    var entry = _dbContext.BlanketPurchaseAgreementLine.Add(line);
                    _dbContext.SaveChanges();
                    entry.State = EntityState.Detached;
                }

                _dbContext.SaveChanges();
                return Get(dbAgreement.AgreementId);
            }
            else if (agreement.AgreementType == AgreementType.Contract) //CPA
            {
                ICollection<QuantityItems> items = new List<QuantityItems>();
                ContractPurchaseAgreementDetails details;
                try
                {
                    foreach (var item in agreement.Items)
                    {
                        items.Add(item.ToObject<QuantityItems>());
                    }

                    details = agreement.Details.ToObject<ContractPurchaseAgreementDetails>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return SupplyResponse.BadRequest("Request Format Fail");
                }

                var dbLine = new List<ContractPurchaseAgreementLine>();

                foreach (var item in items)
                {
                    var dbItem = _dbContext.Item.SingleOrDefault(p => item.SupplierItemId == p.SupplierItemId);
                    if (dbItem == null)
                    {
                        return SupplyResponse.NotFound("supplier item", item.SupplierItemId);
                    }

                    dbLine.Add(new ContractPurchaseAgreementLine()
                    {
                        ItemId = dbItem.Id,
                    });
                }


                //Remove Details
                var _dbDetails = _dbContext.ContractPurchaseAgreementDetails.AsNoTracking()
                    .SingleOrDefault(p => p.AgreementId == _dbAgreement.AgreementId);
                _dbContext.Remove(_dbDetails);


                //Remove Lines
                var lines = _dbContext.ContractPurchaseAgreementLine.Select(p => p)
                    .Where(p => p.AgreementId == _dbAgreement.AgreementId);

                foreach (var line in lines)
                {
                    _dbContext.Remove(line);
                }

                _dbContext.SaveChanges();


                //Create a new replacement agreement
                var dbAgreement = new Agreement
                {
                    AgreementType = AgreementType.Contract,
                    Currency = agreement.Currency,
                    StartDate = agreement.StartDate,
                    ExpiryDate = agreement.ExpiryDate,
                    SupplierId = agreement.SupplierId,
                    CreateBy = _dbAgreement.CreateBy,
                    AgreementId = _dbAgreement.AgreementId,
                    TermsAndCondition = agreement.TermsAndCondition
                };

                //Update Agreement
                _dbContext.Attach(dbAgreement).State = EntityState.Modified;


                //Add back new details
                details.AgreementId = _dbAgreement.AgreementId;
                _dbContext.ContractPurchaseAgreementDetails.Add(details);
                _dbContext.SaveChanges();

                //Add back lines
                foreach (var line in dbLine)
                {
                    line.AgreementId = _dbAgreement.AgreementId;
                    var entry = _dbContext.ContractPurchaseAgreementLine.Add(line);
                    entry.State = EntityState.Detached;
                }

                _dbContext.SaveChanges();

                return Get(_dbAgreement.AgreementId);
            }
            else if (agreement.AgreementType == AgreementType.Planned)
            {
                ICollection<QuantityItems> items = new List<QuantityItems>();
                PlannedPurchaseAgreementDetails details;
                try
                {
                    foreach (var item in agreement.Items)
                    {
                        items.Add(item.ToObject<QuantityItems>());
                    }

                    details = agreement.Details.ToObject<PlannedPurchaseAgreementDetails>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return SupplyResponse.BadRequest("Request Format Fail");
                }

                var dbLine = new Dictionary<string, PlannedPurchaseAgreementLine>();

                foreach (var item in items)
                {
                    if (item.Quantity <= 0)
                    {
                        return SupplyResponse.BadRequest($"Item {item.SupplierItemId} has a zero or negative quantity");
                    }

                    var dbItem = _dbContext.Item.SingleOrDefault(p => item.SupplierItemId == p.SupplierItemId);
                    if (dbItem == null)
                    {
                        return SupplyResponse.NotFound("supplier item", item.SupplierItemId);
                    }

                    if (dbLine.ContainsKey(item.SupplierItemId))
                    {
                        return SupplyResponse.DuplicateEntry("Request Item", item.SupplierItemId);
                    }

                    dbLine[item.SupplierItemId] = new PlannedPurchaseAgreementLine
                    {
                        ItemId = dbItem.Id,
                        Quantity = item.Quantity,

                        Price = item.Price,
                        Unit = item.Unit
                    };
                }

                //Remove Details
                var _dbDetails = _dbContext.PlannedPurchaseAgreementDetails.AsNoTracking()
                    .SingleOrDefault(p => p.AgreementId == _dbAgreement.AgreementId);
                _dbContext.Remove(_dbDetails);


                //Remove Lines
                var lines = _dbContext.PlannedPurchaseAgreementLine.Select(p => p)
                    .Where(p => p.AgreementId == _dbAgreement.AgreementId);

                foreach (var line in lines)
                {
                    _dbContext.Remove(line);
                }

                _dbContext.SaveChanges();

                var dbAgreement = new Agreement
                {
                    AgreementType = AgreementType.Planned,
                    Currency = agreement.Currency,
                    StartDate = agreement.StartDate,
                    ExpiryDate = agreement.ExpiryDate,
                    SupplierId = agreement.SupplierId,
                    CreateBy = dbUser.UserId,
                    AgreementId = _dbAgreement.AgreementId,
                    TermsAndCondition = agreement.TermsAndCondition
                };
                _dbContext.Attach(dbAgreement).State = EntityState.Modified;
                _dbContext.SaveChanges();

                details.AgreementId = _dbAgreement.AgreementId;
                _dbContext.PlannedPurchaseAgreementDetails.Add(details);
                _dbContext.SaveChanges();

                foreach (var line in dbLine.Values)
                {
                    line.AgreementId = _dbAgreement.AgreementId;
                    var entry = _dbContext.PlannedPurchaseAgreementLine.Add(line);
                    entry.State = EntityState.Detached;
                }

                _dbContext.SaveChanges();

                return Get(dbAgreement.AgreementId);
            }

            return SupplyResponse.NotFound("Agreement Type", agreement.AgreementType + "");
        }


        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id)
        {
            var agreement = _dbContext.Agreement.SingleOrDefault(p => p.AgreementId == id);
            if (agreement == null) return SupplyResponse.NotFound("Agreement", id + "");
            _dbContext.Remove(agreement);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}