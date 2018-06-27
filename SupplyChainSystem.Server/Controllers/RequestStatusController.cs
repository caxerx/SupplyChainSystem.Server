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
            request.RequestStatus = requestStatusRequest.Status;
            _dbContext.SaveChanges();
            return SupplyResponse.Ok(request);
        }


    }
}