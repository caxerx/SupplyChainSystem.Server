using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class WarehouseController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public WarehouseController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        // GET api/user
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var items = _dbContext.Warehouse.Select(p => p);
            return SupplyResponse.Ok(items);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var item = _dbContext.Warehouse.SingleOrDefault(p => p.WarehouseId == id);
            if (item == null) return SupplyResponse.NotFound("Warehouse", id + "");
            return SupplyResponse.Ok(item);
        }

        // POST api/user
        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] Warehouse warehouse)
        {
            if (string.IsNullOrWhiteSpace(warehouse.WarehouseName) ||
                string.IsNullOrWhiteSpace(warehouse.WarehouseLocation)) return SupplyResponse.RequiredFieldEmpty();

            var stock = new Stock();
            _dbContext.Stock.Add(stock);
            _dbContext.SaveChanges();

            warehouse.StockId = stock.StockId;
            _dbContext.Warehouse.Add(warehouse);
            _dbContext.SaveChanges();
            return Get(warehouse.WarehouseId);
        }


        // PUT api/user/5
        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] Warehouse warehouse)
        {
            var entity = _dbContext.Warehouse.AsNoTracking().SingleOrDefault(p => p.WarehouseId == id);
            if (entity == null) return Post(warehouse);
            if (string.IsNullOrWhiteSpace(warehouse.WarehouseName) ||
                string.IsNullOrWhiteSpace(warehouse.WarehouseLocation)) return SupplyResponse.RequiredFieldEmpty();

            warehouse.WarehouseId = id;
            warehouse.StockId = entity.StockId;
            _dbContext.Attach(warehouse);
            _dbContext.Entry(warehouse).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id)
        {
            var entity = _dbContext.Warehouse.SingleOrDefault(p => p.WarehouseId == id);
            if (entity == null) return SupplyResponse.NotFound("warehouse", id + "");
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}