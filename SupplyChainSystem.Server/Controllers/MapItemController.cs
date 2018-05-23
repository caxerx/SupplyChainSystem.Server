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
        public ActionResult Get(string id)
        {
            var item = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId.Equals(id));
            if (item == null) return Ok(SupplyResponse.NotFound());
            List<Item> items = new List<Item>();
            foreach (var virtualIdMap in item.VirtualIdMap)
            {
                items.Add(virtualIdMap.Item);
            }
            return Ok(SupplyResponse.Ok(items));
        }

        [HttpPost, Authorize]
        public ActionResult Post([FromBody]VirtualIdMap virtualIdMap)
        {
            var vItem=_dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == virtualIdMap.VirtualItemId);
            var item = _dbContext.Item.SingleOrDefault(p => p.ItemId == virtualIdMap.ItemId);
            if (vItem == null || item == null)
            {
                return Ok(SupplyResponse.NotFound());
            }
            _dbContext.VirtualIdMap.Add(virtualIdMap);
            _dbContext.SaveChanges();
            return Ok(SupplyResponse.Ok());
        }

        
        [HttpDelete, Authorize]
        public ActionResult Delete([FromBody]VirtualIdMap virtualIdMap)
        {
            var entity = _dbContext.VirtualIdMap.SingleOrDefault(p => p.VirtualItemId==virtualIdMap.VirtualItemId && p.ItemId==virtualIdMap.ItemId);
            if (entity == null) return Ok(SupplyResponse.NotFound());
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return Ok(SupplyResponse.Ok());
        }
    }
}