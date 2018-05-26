using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public CategoryController(ProcedurementContext dbContext)
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
            var items = _dbContext.Category.Select(p => p);
            return SupplyResponse.Ok(items);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var item = _dbContext.Category.SingleOrDefault(p => p.CategoryId == id);
            if (item == null) return SupplyResponse.NotFound("categort", id + "");
            return SupplyResponse.Ok(item);
        }

        // POST api/user
        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] Category category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName)) return SupplyResponse.RequiredFieldEmpty();
            _dbContext.Category.Add(category);
            _dbContext.SaveChanges();
            return Get(category.CategoryId);
        }


        // PUT api/user/5
        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] Category category)
        {
            var entity = _dbContext.Category.AsNoTracking().SingleOrDefault(p => p.CategoryId == id);
            if (entity == null) return Post(category);
            if(string.IsNullOrWhiteSpace(category.CategoryName)) return SupplyResponse.RequiredFieldEmpty();
            _dbContext.Attach(category);
            _dbContext.Entry(category).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id)
        {
            var entity = _dbContext.Category.SingleOrDefault(p => p.CategoryId == id);
            if (entity == null) return SupplyResponse.NotFound("categort", id + "");
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}