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
    public class DeliveryNoteController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public DeliveryNoteController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var orders = _dbContext.DeliveryNote.Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem).Include(p => p.Request)
                .ThenInclude(p => p.Restaurant).Select(p => p);

            return SupplyResponse.Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var order = _dbContext.DeliveryNote.Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem).Include(p => p.Request)
                .ThenInclude(p => p.Restaurant)
                .SingleOrDefault(p => p.RequestId == id);
            return order == null ? SupplyResponse.NotFound("Delivery Note", id + "") : SupplyResponse.Ok(order);
        }


        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id,DeliveryNote deliveryNote)
        {
            var order = _dbContext.DeliveryNote.Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem).Include(p => p.Request)
                .ThenInclude(p => p.Restaurant)
                .SingleOrDefault(p => p.RequestId == id);
            if (order == null) return SupplyResponse.NotFound("Purchase Order", id + "");

            order.DeliveryStatus = deliveryNote.DeliveryStatus;

            _dbContext.SaveChanges();


            return Get(id);
        }
    }
}