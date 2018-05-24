using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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


        [HttpGet("{id}"), Authorize]
        public ActionResult Get(string id, bool virtualid)
        {
            if (virtualid)
            {
                var item = _dbContext.VirtualItem.Include(a => a.VirtualIdMap).ThenInclude(b => b.Item)
                    .SingleOrDefault(p => p.VirtualItemId.Equals(id));
                if (item == null) return Ok(SupplyResponse.NotFound());
                List<string> items = new List<string>();
                if (item.VirtualIdMap != null)
                {
                    foreach (var virtualIdMap in item.VirtualIdMap)
                    {
                        items.Add(virtualIdMap.ItemId);
                    }
                }

                return Ok(SupplyResponse.Ok(items));
            }
            else
            {
                var item = _dbContext.Item.Include(a => a.VirtualIdMap).ThenInclude(b => b.VirtualItem)
                    .SingleOrDefault(p => p.ItemId.Equals(id));
                if (item == null) return Ok(SupplyResponse.NotFound());
                List<string> items = new List<string>();
                if (item.VirtualIdMap != null)
                {
                    foreach (var virtualIdMap in item.VirtualIdMap)
                    {
                        items.Add(virtualIdMap.VirtualItemId);
                    }
                }

                return Ok(SupplyResponse.Ok(items));
            }
        }

        [HttpPost("{id}"), Authorize]
        public ActionResult Post(string id, [FromBody] IdRequest idRequest)
        {
            var vItem = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == idRequest.Id);
            var item = _dbContext.Item.SingleOrDefault(p => p.ItemId == id);
            if (vItem == null || item == null)
            {
                return Ok(SupplyResponse.NotFound());
            }

            var virtualIdMap = new VirtualIdMap
            {
                ItemId = item.ItemId,
                VirtualItemId = vItem.VirtualItemId
            };
            if (_dbContext.VirtualIdMap.SingleOrDefault(p =>
                    p.ItemId == virtualIdMap.ItemId && p.VirtualItemId == virtualIdMap.ItemId) !=
                null)
            {
                return Ok(SupplyResponse.Fail("Duplicate Entry"));
            }

            _dbContext.VirtualIdMap.Add(virtualIdMap);
            _dbContext.SaveChanges();
            return Ok(SupplyResponse.Ok());
        }


        [HttpDelete("{id}"), Authorize]
        public ActionResult Delete(string id, [FromBody] IdRequest idRequest)
        {
            var entity =
                _dbContext.VirtualIdMap.SingleOrDefault(p => p.VirtualItemId == idRequest.Id && p.ItemId == id);
            if (entity == null) return Ok(SupplyResponse.NotFound());
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return Ok(SupplyResponse.Ok());
        }
    }
}