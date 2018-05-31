using System;
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
    public class RestaurantManagerController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public RestaurantManagerController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        // GET api/user
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var restaurants = _dbContext.Restaurant.Include(p => p.RestaurantManager).Select(p => p);
            return SupplyResponse.Ok(restaurants);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var restaurant = _dbContext.Restaurant.Include(p => p.RestaurantManager)
                .SingleOrDefault(p => p.RestaurantId == id);
            return SupplyResponse.Ok(restaurant);
        }


        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse AddRestaurantManager(int id, [FromBody] IntIdRequest idRequest)
        {
            if (id == 0 || idRequest.Id == 0) return SupplyResponse.RequiredFieldEmpty();
            var restaurant = _dbContext.Restaurant.SingleOrDefault(p => id == p.RestaurantId);
            var user = _dbContext.User.SingleOrDefault(p => p.UserId == idRequest.Id);
            if (restaurant == null) return SupplyResponse.NotFound("restaurant", id + "");
            if (user == null) return SupplyResponse.NotFound("user", idRequest.Id + "");
            var restaurantManager = new RestaurantManager
            {
                RestaurantId = id,
                UserId = idRequest.Id
            };
            _dbContext.RestaurantManager.Add(restaurantManager);
            _dbContext.SaveChanges();
            return Get(id);
        }

        // POST api/category/{id}/add
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse RemoveRestaurantManager(int id, [FromBody] IntIdRequest idRequest)
        {
            if (id == 0 || idRequest.Id == 0) return SupplyResponse.RequiredFieldEmpty();

            var restaurantManager =
                _dbContext.RestaurantManager.SingleOrDefault(p => p.RestaurantId == id && p.UserId == idRequest.Id);
            if (restaurantManager == null)
                return SupplyResponse.NotFound("restaurant manager", idRequest.Id + "<->" + id);
            _dbContext.RestaurantManager.Remove(restaurantManager);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}