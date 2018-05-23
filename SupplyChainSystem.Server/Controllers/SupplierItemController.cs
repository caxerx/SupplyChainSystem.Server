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
    public class SupplierItemController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public SupplierItemController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }
        /**
         •	Item Management
            o	Create and Edit Item, Category
            o	Virtual ID mapping
         */

        // GET api/user
        [HttpGet, Authorize]
        public ActionResult Get()
        {
            var items = _dbContext.Item.Select(p => p);
            return Ok(SupplyResponse.Ok(items));
        }

        // GET api/user/3
        [HttpGet("{id}"), Authorize]
        public ActionResult Get(string id)
        {
            var item = _dbContext.Item.SingleOrDefault(p => p.ItemId.Equals(id));
            if (item == null) return NoContent();
            return Ok(SupplyResponse.Ok(item));
        }

        // POST api/user
        [HttpPost, Authorize]
        public ActionResult Post([FromBody] Item item)
        {
            if (_dbContext.Item.Contains(item))
            {
                return Ok(SupplyResponse.Fail("Duplicated entry"));
            }
            _dbContext.Item.Add(item);
            _dbContext.SaveChanges();
            return Get(item.ItemId);
        }

        // PUT api/user/5
        [HttpPut("{id}"), Authorize]
        public ActionResult Put(string id, [FromBody] Item item)
        {
            var entity = _dbContext.Item.AsNoTracking().SingleOrDefault(p => p.ItemId == id);
            if (entity == null) return Post(item);

            _dbContext.Attach(item);
            _dbContext.Entry(item).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/user/5
        [HttpDelete("{id}"), Authorize]
        public ActionResult Delete(string id)
        {
            var entity = _dbContext.Item.SingleOrDefault(p => p.ItemId == id);
            if (entity == null) return BadRequest();
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return Ok(SupplyResponse.Ok());
        }
    }
}