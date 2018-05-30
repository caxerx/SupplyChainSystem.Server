using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SupplierController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public SupplierController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/supplier
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var suppliers = _dbContext.Supplier.Select(p => p);
            return SupplyResponse.Ok(suppliers);
        }

        // GET api/supplier/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var supplier = _dbContext.Supplier.SingleOrDefault(p => p.SupplierId == id);
            if (supplier == null) return SupplyResponse.NotFound("supplier", id + "");
            return SupplyResponse.Ok(supplier);
        }

        // POST api/supplier
        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] Supplier supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
                return SupplyResponse.BadRequest("Required Field is Empty");
            _dbContext.Supplier.Add(supplier);
            _dbContext.SaveChanges();
            return Get(supplier.SupplierId);
        }

        // PUT api/supplier/5
        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] Supplier supplier)
        {
            var entity = _dbContext.Supplier.AsNoTracking().SingleOrDefault(p => p.SupplierId == id);
            if (entity == null) return SupplyResponse.NotFound("supplier", id + "");
            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
                return SupplyResponse.BadRequest("Required Field is Empty");
            supplier.SupplierId = id;
            _dbContext.Attach(supplier);
            _dbContext.Entry(supplier).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/supplier/5
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id)
        {
            var entity = _dbContext.Supplier.SingleOrDefault(p => p.SupplierId == id);
            if (entity == null) return SupplyResponse.NotFound("supplier", id + "");
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}