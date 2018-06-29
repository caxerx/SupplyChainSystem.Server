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
    public class DespatchInstructionController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public DespatchInstructionController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        public SupplyResponse Get()
        {
            var orders = _dbContext.DespatchInstruction.Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem).Include(p => p.Request)
                .ThenInclude(p => p.Restaurant).Select(p => p);

            return SupplyResponse.Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var order = _dbContext.DespatchInstruction.Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem).Include(p => p.Request)
                .ThenInclude(p => p.Restaurant)
                .SingleOrDefault(p => p.DespatchInstructionId == id);
            return order == null ? SupplyResponse.NotFound("Despatch Instruction", id + "") : SupplyResponse.Ok(order);
        }


        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id)
        {
            var order = _dbContext.DespatchInstruction.Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem).Include(p => p.Request)
                .ThenInclude(p => p.Restaurant)
                .SingleOrDefault(p => p.DespatchInstructionId == id);
            if (order == null)
            {
                return SupplyResponse.NotFound("Purchase Order", id + "");
            }

            order.DespatchInstructionStatus = 1;

            _dbContext.SaveChanges();

            _dbContext.DeliveryNote.Add(new DeliveryNote
            {
                CreateTime = DateTime.Now,
                RequestId = order.RequestId
            });

            _dbContext.SaveChanges();

            return Get(id);
        }
    }
}