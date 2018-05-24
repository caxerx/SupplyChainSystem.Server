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
        public ActionResult Get()
        {
            var items = _dbContext.Category.Select(p => p);
            return Ok(SupplyResponse.Ok(items));
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult Get(int id)
        {
            var item = _dbContext.Category.SingleOrDefault(p => p.CategoryId == id);
            if (item == null) return Ok(SupplyResponse.NotFound());
            return Ok(SupplyResponse.Ok(item));
        }

        // POST api/user
        [HttpPost]
        [Authorize]
        public ActionResult Post([FromBody] Category category)
        {
            _dbContext.Category.Add(category);
            _dbContext.SaveChanges();
            return Get(category.CategoryId);
        }


        // PUT api/user/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Put(int id, [FromBody] Category category)
        {
            var entity = _dbContext.Category.AsNoTracking().SingleOrDefault(p => p.CategoryId == id);
            if (entity == null) return Post(category);

            _dbContext.Attach(category);
            _dbContext.Entry(category).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var entity = _dbContext.Category.SingleOrDefault(p => p.CategoryId == id);
            if (entity == null) return BadRequest();
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return Ok(SupplyResponse.Ok());
        }
    }
}