﻿using System;
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
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem).Select(p => p);

            return SupplyResponse.Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var order = _dbContext.DespatchInstruction.Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem)
                .SingleOrDefault(p => p.RequestId == id);
            return order == null ? SupplyResponse.NotFound("Despatch Instruction", id + "") : SupplyResponse.Ok(order);
        }


        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, DespatchInstruction orderStatus)
        {
            var order = _dbContext.DespatchInstruction.Include(p => p.Request)
                .ThenInclude(p => p.RequestItem).ThenInclude(p => p.VirtualItem)
                .SingleOrDefault(p => p.RequestId == id);
            if (order == null)
            {
                return SupplyResponse.NotFound("Purchase Order", id + "");
            }

            order.DeliveryStatus = orderStatus.DeliveryStatus;
            _dbContext.SaveChanges();


            return Get(id);
        }
    }
}