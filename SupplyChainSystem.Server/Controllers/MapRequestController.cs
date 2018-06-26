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

        // GET api/user
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var requestQueue = _dbContext.Request.Include(p => p.RequestItem).ThenInclude(p => p.VirtualItem)
                .ThenInclude(p => p.VirtualIdMap).ThenInclude(p => p.Item)
                .Where(p => p.RequestStatus == RequestStatus.WaitingForProcess)
                .Select(p => p);
            foreach (var request in requestQueue)
            {
                request.RequestStatus = RequestStatus.WaitingForProcess;
                _dbContext.SaveChanges();
            }

            foreach (var request in requestQueue)
            {
                //find all items in request
                var vItems = request.RequestItem.Select(p => p.VirtualItem).ToList();

                //search for active blanket
                var blanketAgreements = _dbContext.Agreement.Where(p =>
                        p.AgreementType == AgreementType.Blanket && DateTime.Now < p.ExpiryDate)
                    .Select(p => p)
                    .Include(p => p.BlanketPurchaseAgreementDetails)
                    .Include(p => p.BlanketPurchaseAgreementLines).ThenInclude(p => p.Item);

                var matchedAgreement = new List<Agreement>();

                //search any agreement that contains all items in Request status
                foreach (var agreement in blanketAgreements)
                {
                    var agreementVItems = agreement.BlanketPurchaseAgreementLines.Select(p =>
                        p.Item.VirtualIdMap.Select(q => q.VirtualItem)).Aggregate((ta, i) =>
                    {
                        var ls = new List<VirtualItem>();
                        ls.AddRange(ta);
                        ls.AddRange(i);
                        return ls;
                    });

                    if (!vItems.Any(p => agreementVItems.Contains(p)))
                    {
                        matchedAgreement.Add(agreement);
                    }
                }

                var matchedAgreementWithLine = new Dictionary<Agreement, dynamic>();
                //find promised qty & min qty
                foreach (var agreement in matchedAgreement)
                {
                    var amount = 0d;
                    var lines = agreement.BlanketPurchaseAgreementLines;
                    var matchedLines = new List<dynamic>();
                    foreach (var requestItem in request.RequestItem)
                    {
                        var vItem = requestItem.VirtualItem;
                        var matchedLine = lines.SingleOrDefault(line =>
                        {
                            if (line.Item.VirtualIdMap.Select(p => p.VirtualItem).Contains(vItem) &&
                                requestItem.Quantity > line.MinimumQuantity &&
                                line.UsedQuantity + requestItem.Quantity < line.PromisedQuantity)
                            {
                                amount += line.Price * requestItem.Quantity;
                                return true;
                            }

                            return false;
                        });
                        if (matchedLine == null)
                        {
                            amount = 0d;
                            matchedLines.Clear();
                            break;
                        }

                        matchedLines.Add(new {requestItem, matchedLine});
                    }

                    var details = agreement.BlanketPurchaseAgreementDetails;
                    if (matchedLines.Any() && details.AmountUsed + amount <= details.AmountAgreed)
                    {
                        matchedAgreementWithLine.Add(agreement, matchedLines);
                    }
                }

                return SupplyResponse.Ok(matchedAgreementWithLine);
            }

            return SupplyResponse.Fail("FuckedUp", "nothing fucking");
        }
    }
}