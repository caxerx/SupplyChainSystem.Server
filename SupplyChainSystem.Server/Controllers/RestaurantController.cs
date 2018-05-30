using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RestaurantController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public RestaurantController(ProcedurementContext dbContext)
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
            var items = _dbContext.Restaurant.Select(p => p);
            return SupplyResponse.Ok(items);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var item = _dbContext.Restaurant.SingleOrDefault(p => p.RestaurantId == id);
            if (item == null) return SupplyResponse.NotFound("Restaurant", id + "");
            return SupplyResponse.Ok(item);
        }

        // POST api/user
        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] Restaurant restaurant)
        {
            if (string.IsNullOrWhiteSpace(restaurant.RestaurantName) ||
                string.IsNullOrWhiteSpace(restaurant.RestaurantLocation) ||
                restaurant.RestaurantTypeId == 0) return SupplyResponse.RequiredFieldEmpty();
            var stock = _dbContext.Stock.Add(new Stock());
            restaurant.StockId = stock.Entity.StockId;
            _dbContext.Restaurant.Add(restaurant);
            _dbContext.SaveChanges();
            return Get(restaurant.RestaurantId);
        }


        // PUT api/user/5
        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] Restaurant restaurant)
        {
            var entity = _dbContext.Restaurant.AsNoTracking().SingleOrDefault(p => p.RestaurantId == id);
            if (entity == null) return Post(restaurant);
            if (string.IsNullOrWhiteSpace(restaurant.RestaurantName) ||
                string.IsNullOrWhiteSpace(restaurant.RestaurantLocation) ||
                restaurant.RestaurantTypeId == 0) return SupplyResponse.RequiredFieldEmpty();
            restaurant.RestaurantId = id;
            _dbContext.Attach(restaurant);
            _dbContext.Entry(restaurant).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id)
        {
            var entity = _dbContext.Restaurant.SingleOrDefault(p => p.RestaurantId == id);
            if (entity == null) return SupplyResponse.NotFound("restaurant", id + "");
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}