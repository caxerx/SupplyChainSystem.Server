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
                var vItems = request.RequestItem.Select(p => p.VirtualItem);

                //search for blanket
                var blanketAgreements = _dbContext.Agreement.Where(p => p.AgreementType == AgreementType.Blanket)
                    .Select(p => p)
                    .Include(p => p.BlanketPurchaseAgreementDetails)
                    .Include(p => p.BlanketPurchaseAgreementLines).ThenInclude(p => p.Item);
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
                }


            }

            return SupplyResponse.Ok();
        }
    }
}