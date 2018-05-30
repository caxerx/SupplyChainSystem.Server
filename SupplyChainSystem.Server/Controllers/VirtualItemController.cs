using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class VirtualItemController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public VirtualItemController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }
        /**
         •	Item Management
            o	Create and Edit Item, Category
            o	Virtual ID mapping
         */

        // GET api/user
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var items = _dbContext.VirtualItem.Select(p => p);
            return SupplyResponse.Ok(items);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(string id)
        {
            var item = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId.Equals(id));
            if (item == null) return SupplyResponse.NotFound("virtual item", id);
            return SupplyResponse.Ok(item);
        }

        // POST api/user
        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] VirtualItem item)
        {
            if (string.IsNullOrWhiteSpace(item.VirtualItemName) || string.IsNullOrWhiteSpace(item.VirtualItemId))
                return SupplyResponse.RequiredFieldEmpty();
            if (_dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == item.VirtualItemId) != null)
                return SupplyResponse.DuplicateEntry("item", item.VirtualItemId);
            _dbContext.VirtualItem.Add(item);
            _dbContext.SaveChanges();
            return Get(item.VirtualItemId);
        }

        // PUT api/user/5
        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(string id, [FromBody] VirtualItem item)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(item.VirtualItemName) ||
                string.IsNullOrWhiteSpace(item.VirtualItemId))
                return SupplyResponse.RequiredFieldEmpty();

            var entity = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId.Equals(id));
            if (entity == null) return Post(item);

            if (entity.VirtualItemId != item.VirtualItemId &&
                _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == item.VirtualItemId) != null)
                return SupplyResponse.DuplicateEntry("virtual item", item.VirtualItemId);

            item.Id = entity.Id;

            var entry = _dbContext.Entry(entity);
            entry.CurrentValues.SetValues(item);
            entry.State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(item.VirtualItemId);
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(string id)
        {
            var entity = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId.Equals(id));
            if (entity == null) return SupplyResponse.NotFound("virtual item", id + "");
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}