﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MapItemController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public MapItemController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(string id, bool supplieritem)
        {
            if (supplieritem)
            {
                var item = _dbContext.Item.Include(a => a.VirtualIdMap).ThenInclude(b => b.VirtualItem)
                    .SingleOrDefault(p => p.SupplierItemId.Equals(id));
                if (item == null) return SupplyResponse.NotFound("item", id);
                var items = new List<string>();
                if (item.VirtualIdMap != null)
                    foreach (var virtualIdMap in item.VirtualIdMap)
                        items.Add(virtualIdMap.VirtualItem.VirtualItemId);

                return SupplyResponse.Ok(items);
            }
            else
            {
                var item = _dbContext.VirtualItem.Include(a => a.VirtualIdMap).ThenInclude(b => b.Item)
                    .SingleOrDefault(p => p.VirtualItemId.Equals(id));
                if (item == null) return SupplyResponse.NotFound("virtual item", id);
                var items = new List<string>();
                if (item.VirtualIdMap != null)
                    foreach (var virtualIdMap in item.VirtualIdMap)
                        items.Add(virtualIdMap.Item.SupplierItemId);

                return SupplyResponse.Ok(items);
            }
        }

        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse Post(string id, [FromBody] IdRequest idRequest)
        {
            if (idRequest == null || string.IsNullOrWhiteSpace(idRequest.Id))
                return SupplyResponse.RequiredFieldEmpty();
            var vItem = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == idRequest.Id);
            var item = _dbContext.Item.SingleOrDefault(p => p.SupplierItemId == id);
            if (item == null) return SupplyResponse.NotFound("item", id);
            if (vItem == null) return SupplyResponse.NotFound("virtual item", idRequest.Id);

            if (_dbContext.VirtualIdMap.SingleOrDefault(p =>
                    p.ItemId == item.Id && p.VirtualItemId == vItem.Id) != null)
                return SupplyResponse.DuplicateEntry("virtual map", $"{id}<->{idRequest.Id}");

            _dbContext.VirtualIdMap.Add(new VirtualIdMap {ItemId = item.Id, VirtualItemId = vItem.Id});
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }


        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(string id, [FromBody] IdRequest idRequest)
        {
            var entity =
                _dbContext.VirtualIdMap.SingleOrDefault(p =>
                    p.VirtualItem.VirtualItemId == idRequest.Id && p.Item.SupplierItemId == id);
            if (entity == null) return SupplyResponse.NotFound("virtual map", $"{id}<->{idRequest.Id}");
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}