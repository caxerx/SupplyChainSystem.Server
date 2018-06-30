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
    public class RequestStatusController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public RequestStatusController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse UpdateRequestStatus(int id, [FromBody] RequestStatusRequest requestStatusRequest)
        {
            var request = _dbContext.Request.SingleOrDefault(p => p.RequestId == id);

            if (request == null)
            {
                return SupplyResponse.NotFound("Request", id + "");
            }

            if (requestStatusRequest.Status == RequestStatus.Cancelled)
            {
                request.RequestStatus = requestStatusRequest.Status;
                _dbContext.SaveChanges();
                return SupplyResponse.Ok(request);
            }

            if (requestStatusRequest.Status == RequestStatus.Delivered)
            {
                var map = _dbContext.RequestMap.Include(p => p.Agreement).Include(p => p.DespatchInstruction)
                    .Include(p => p.Request).FirstOrDefault(p => p.RequestId == id);
                if (map == null)
                {
                    return SupplyResponse.Fail("Unprocessed Request", "The selected request not processed yet.");
                }

                switch (map.MapType)
                {
                    case MapType.BPA:
                        request.RequestStatus = requestStatusRequest.Status;
                        var release = _dbContext.BlanketRelease.FirstOrDefault(p => p.RequestId == id);
                        release.PurchaseOrderStatus = 1;
                        break;
                    case MapType.Contract:
                        request.RequestStatus = requestStatusRequest.Status;
                        var order = _dbContext.StandardPurchaseOrder.FirstOrDefault(p => p.RequestId == id);
                        order.PurchaseOrderStatus = 1;
                        break;
                    case MapType.Warehouse:
                        request.RequestStatus = requestStatusRequest.Status;
                        var dn = _dbContext.DeliveryNote.FirstOrDefault(p => p.RequestId == id);
                        if (dn != null)
                        {
                            dn.DeliveryStatus = 1;
                        }
                        else
                        {
                            var di = _dbContext.DespatchInstruction.FirstOrDefault(p => p.RequestId == id);
                            di.DespatchInstructionStatus = 1;
                        }

                        break;
                }

                _dbContext.SaveChanges();
                return SupplyResponse.Ok();
            }

            return SupplyResponse.Fail("Unauthorized Operation", "Illegal access");
        }
    }
}