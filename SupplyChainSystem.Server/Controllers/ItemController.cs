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
    public class ItemController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public ItemController(ProcedurementContext dbContext)
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
            var items = _dbContext.VirtualItem.Select(p => p);
            return Ok(items);
        }

        // GET api/user/3
        [HttpGet("{id}"), Authorize]
        public ActionResult Get(string id)
        {
            var item = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId.Equals(id));
            if (item == null) return NoContent();
            return Ok(item);
        }

        // POST api/user
        [HttpPost, Authorize]
        public ActionResult Post([FromBody] VirtualItem virtualItem)
        {
            _dbContext.VirtualItem.Add(virtualItem);
            _dbContext.SaveChanges();
            return Get(virtualItem.VirtualItemId);
        }

        // PUT api/user/5
        [HttpPut("{id}"), Authorize]
        public ActionResult Put(string id, [FromBody] VirtualItem virtualItem)
        {
            var entity = _dbContext.VirtualItem.AsNoTracking().SingleOrDefault(p => p.VirtualItemId == id);
            if (entity == null) return Post(virtualItem);

            _dbContext.Attach(virtualItem);
            _dbContext.Entry(virtualItem).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/user/5
        [HttpDelete("{id}"), Authorize]
        public ActionResult Delete(string id)
        {
            var entity = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == id);
            if (entity == null) return BadRequest();
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}