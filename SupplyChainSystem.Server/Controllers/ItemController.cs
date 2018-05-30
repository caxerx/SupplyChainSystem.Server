using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var items = _dbContext.Item.Select(p => p);
            return SupplyResponse.Ok(items);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(string id)
        {
            var item = _dbContext.Item.SingleOrDefault(p => p.SupplierItemId.Equals(id));
            if (item == null) return SupplyResponse.NotFound("item", id);
            return SupplyResponse.Ok(item);
        }

        // POST api/user
        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] Item item)
        {
            if (string.IsNullOrWhiteSpace(item.ItemName) || item.SupplierId == 0 ||
                string.IsNullOrWhiteSpace(item.SupplierItemId))
                return SupplyResponse.RequiredFieldEmpty();
            if (_dbContext.Item.SingleOrDefault(p => p.SupplierItemId == item.SupplierItemId) != null)
                return SupplyResponse.DuplicateEntry("item", item.SupplierItemId);
            if (_dbContext.Supplier.SingleOrDefault(p => p.SupplierId == item.SupplierId) == null)
                return SupplyResponse.NotFound("supplier", item.SupplierId + "");
            _dbContext.Item.Add(item);
            _dbContext.SaveChanges();
            return Get(item.SupplierItemId);
        }

        // PUT api/user/5
        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(string id, [FromBody] Item item)
        {
            if (item.SupplierId == 0 || string.IsNullOrWhiteSpace(item.ItemName) ||
                string.IsNullOrWhiteSpace(item.SupplierItemId + ""))
                return SupplyResponse.RequiredFieldEmpty();

            if (_dbContext.Supplier.SingleOrDefault(p => p.SupplierId == item.SupplierId) == null)
                return SupplyResponse.NotFound("supplier", item.SupplierId + "");

            var entity = _dbContext.Item.SingleOrDefault(p => p.SupplierItemId == id);
            if (entity == null) return Post(item);

            if (entity.SupplierItemId != item.SupplierItemId &&
                _dbContext.Item.SingleOrDefault(p => p.SupplierItemId == item.SupplierItemId) != null)
                return SupplyResponse.DuplicateEntry("item", item.SupplierItemId);

            item.Id = entity.Id;

            var entry = _dbContext.Entry(entity);
            entry.CurrentValues.SetValues(item);
            entry.State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(item.SupplierItemId);
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(string id)
        {
            var entity = _dbContext.Item.SingleOrDefault(p => p.SupplierItemId.Equals(id));
            if (entity == null) return SupplyResponse.NotFound("item", id + "");
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}