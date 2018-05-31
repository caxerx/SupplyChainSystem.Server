using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RestaurantTypeController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public RestaurantTypeController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var restaurantTypes = _dbContext.RestaurantType.Select(p => p);
            return SupplyResponse.Ok(restaurantTypes);
        }

        [HttpPost]
        [Authorize]
        public SupplyResponse AddRestaurantType([FromBody] NameRequest nameRequest)
        {
            if (nameRequest.Name == null) return SupplyResponse.RequiredFieldEmpty();
            var restaurantType = new RestaurantType
            {
                RestaurantTypeName = nameRequest.Name
            };

            var entity = _dbContext.RestaurantType.Add(restaurantType);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok(entity.Entity);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse RemoveRestaurantType(int id)
        {
            if (id == 0) return SupplyResponse.RequiredFieldEmpty();

            var restaurantType =
                _dbContext.RestaurantType.SingleOrDefault(p => p.RestaurantTypeId == id);
            if (restaurantType == null)
                return SupplyResponse.NotFound("restaurant type", "" + id);
            _dbContext.RestaurantType.Remove(restaurantType);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}