using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ScheduleReleaseController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public ScheduleReleaseController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        public SupplyResponse Get()
        {
            var orders = _dbContext.ScheduleRelease.Include(p => p.Agreement)
                .ThenInclude(p => p.ContractPurchaseAgreementDetails).Include(p => p.Agreement)
                .ThenInclude(p => p.ContractPurchaseAgreementLines).ThenInclude(p => p.Item).Select(p => p);

            return SupplyResponse.Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var order = _dbContext.ScheduleRelease.Include(p => p.Agreement)
                .ThenInclude(p => p.ContractPurchaseAgreementDetails).Include(p => p.Agreement)
                .ThenInclude(p => p.ContractPurchaseAgreementLines).ThenInclude(p => p.Item)
                .SingleOrDefault(p => p.OrderId == id);
            return order == null ? SupplyResponse.NotFound("Schedule Release", id + "") : SupplyResponse.Ok(order);
        }

    }
}