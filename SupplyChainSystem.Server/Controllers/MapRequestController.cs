using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MapRequestController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public MapRequestController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        [Authorize]
        public SupplyResponse Post()
        {
            var requestQueue = _dbContext.Request.Include(p => p.RequestItem).ThenInclude(p => p.VirtualItem)
                .ThenInclude(p => p.VirtualIdMap).ThenInclude(p => p.Item)
                .Where(p => p.RequestStatus == RequestStatus.WaitingForProcess)
                .Select(p => p);

            foreach (var request in requestQueue)
            {
                _dbContext.Request.SingleOrDefault(p => p.RequestId == request.RequestId).RequestStatus =
                    RequestStatus.Processing;
                _dbContext.SaveChanges();
                //find all items in request
                var vItems = request.RequestItem.Select(p => p.VirtualItem).ToList();

                //search for active blanket
                var blanketAgreements = _dbContext.Agreement.Where(p =>
                        p.AgreementType == AgreementType.Blanket && DateTime.Now < p.ExpiryDate)
                    .Select(p => p)
                    .Include(p => p.BlanketPurchaseAgreementDetails)
                    .Include(p => p.BlanketPurchaseAgreementLines).ThenInclude(p => p.Item);

                var itemMatchedAgreement = new List<Agreement>();

                //search any agreement that contains all items in Request status
                foreach (var agreement in blanketAgreements)
                {
                    Console.WriteLine(
                        $"mapping {request.RequestId}, testing for {agreement.AgreementId}:contains item");
                    var agreementVItems = agreement.BlanketPurchaseAgreementLines.Select(p =>
                        (p.Item.VirtualIdMap ?? new List<VirtualIdMap>()).Select(q => q.VirtualItem)).Aggregate(
                        (ta, i) =>
                        {
                            var ls = new List<VirtualItem>();
                            ls.AddRange(ta);
                            ls.AddRange(i);
                            return ls;
                        });

                    if (vItems.All(p => agreementVItems.Contains(p)))
                    {
                        Console.WriteLine(
                            $"mapping {request.RequestId}, {agreement.AgreementId} contains all needed item");
                        itemMatchedAgreement.Add(agreement);
                    }
                }

                var matchedAgreementList = new List<Agreement>();
                //find promised qty & min qty
                foreach (var agreement in itemMatchedAgreement)
                {
                    Console.WriteLine(
                        $"mapping {request.RequestId}, testing for {agreement.AgreementId}:qty and price");
                    var amount = 0d;
                    var lines = agreement.BlanketPurchaseAgreementLines;
                    var matchedLines = new List<dynamic>();
                    foreach (var requestItem in request.RequestItem)
                    {
                        var vItem = requestItem.VirtualItem;
                        var matchedLine = lines.SingleOrDefault(line =>
                        {
                            if ((line.Item.VirtualIdMap ?? new List<VirtualIdMap>()).Select(p => p.VirtualItem)
                                .Contains(vItem) &&
                                requestItem.Quantity >= line.MinimumQuantity &&
                                line.UsedQuantity + requestItem.Quantity < line.PromisedQuantity)
                            {
                                Console.WriteLine(
                                    $"mapping {request.RequestId}, a line in agreement {agreement.AgreementId} match");
                                amount += line.Price * requestItem.Quantity;
                                return true;
                            }

                            return false;
                        });
                        if (matchedLine == null)
                        {
                            Console.WriteLine(
                                $"mapping {request.RequestId}, can't find a line to match request item in agreement {agreement.AgreementId}");
                            amount = 0d;
                            matchedLines.Clear();
                            break;
                        }

                        matchedLines.Add(new {requestItem, matchedLine});
                    }

                    var details = agreement.BlanketPurchaseAgreementDetails;
                    if (matchedLines.Any() && details.AmountUsed + amount <= details.AmountAgreed)
                    {
                        matchedAgreementList.Add(agreement);
                        Console.WriteLine($"mapping {request.RequestId}, {agreement.AgreementId} match");
                    }
                    else
                    {
                        Console.WriteLine($"mapping {request.RequestId}, {agreement.AgreementId} not match");
                        Console.WriteLine(
                            $"{details.AmountUsed},{amount},{details.AmountUsed + amount},{details.AmountAgreed}");
                    }
                }


                if (matchedAgreementList.Any())
                {
                    Console.WriteLine($"Adding request map {request.RequestId}");
                    _dbContext.RequestMap.Add(new RequestMap
                    {
                        AgreementId = matchedAgreementList.OrderBy(p => p.ExpiryDate).First().AgreementId,
                        MapType = MapType.BPA,
                        RequestId = request.RequestId
                    });
                    _dbContext.Request.SingleOrDefault(p => p.RequestId == request.RequestId).RequestStatus =
                        RequestStatus.Ordered;
                    _dbContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"No match for request {request.RequestId}");
                    _dbContext.Request.SingleOrDefault(p => p.RequestId == request.RequestId).RequestStatus =
                        RequestStatus.Failed;
                    _dbContext.SaveChanges();
                }
            }


            return SupplyResponse.Ok();
            //return SupplyResponse.Fail("FuckedUp", "nothing fucking");
        }


        /*
        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse Post(string id)
        {
        }
        */
    }
}