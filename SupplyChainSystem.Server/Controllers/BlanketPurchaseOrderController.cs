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
    public class BlanketPurchaseOrderController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public BlanketPurchaseOrderController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        public SupplyResponse Get()
        {
            var orders = _dbContext.BlanketRelease.Include(p => p.Agreement).ThenInclude(p=>p.Supplier).Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).Include(p => p.BlanketReleaseLine).ThenInclude(p=>p.Item).Include(p => p.Request)
                .ThenInclude(p => p.Restaurant)
                .Select(p => p);

            return SupplyResponse.Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var order = _dbContext.BlanketRelease.Include(p => p.Agreement).ThenInclude(p => p.Supplier).Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).Include(p => p.BlanketReleaseLine).ThenInclude(p => p.Item).Include(p => p.Request)
                .ThenInclude(p => p.Restaurant)
                .SingleOrDefault(p => p.OrderId == id);
            return order == null ? SupplyResponse.NotFound("Purchase Order", id + "") : SupplyResponse.Ok(order);
        }


        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody]BlanketRelease orderStatus)
        {
            var order = _dbContext.BlanketRelease.SingleOrDefault(p => p.OrderId == id);

            if (order == null)
            {
                return SupplyResponse.NotFound("Purchase Order", id + "");
            }

            order.PurchaseOrderStatus = orderStatus.PurchaseOrderStatus;
            _dbContext.SaveChanges();

            return Get(id);
        }
    }
}