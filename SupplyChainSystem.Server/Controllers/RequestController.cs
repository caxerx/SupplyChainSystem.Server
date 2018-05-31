using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RequestController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public RequestController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        public SupplyResponse Get()
        {
            var currentUser = HttpContext.User;
            var dbUser =
                _dbContext.User.Include(p => p.RestaurantManager).ThenInclude(p => p.Restaurant)
                    .SingleOrDefault(p => currentUser.FindFirst(ClaimTypes.Name).Value.Equals(p.UserName));
            if (dbUser == null) return SupplyResponse.Fail("Unauthorize", "Your are not the user in the system.");
            var restaurantManager = dbUser.RestaurantManager;
            if (restaurantManager == null)
                return SupplyResponse.Fail("Unauthorize", "Your are not the restaurant manager.");
            var restaurantId = restaurantManager.Restaurant.RestaurantId;
            var requests = _dbContext.Request.Where(p => p.RestaurantId == restaurantId).Select(p => p);
            return SupplyResponse.Ok(requests);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var currentUser = HttpContext.User;
            var dbUser =
                _dbContext.User.Include(p => p.RestaurantManager).ThenInclude(p => p.Restaurant)
                    .SingleOrDefault(p => currentUser.FindFirst(ClaimTypes.Name).Value.Equals(p.UserName));
            if (dbUser == null) return SupplyResponse.Fail("Unauthorize", "Your are not the user in the system.");
            var restaurantManager = dbUser.RestaurantManager;
            if (restaurantManager == null)
                return SupplyResponse.Fail("Unauthorize", "Your are not the restaurant manager.");
            var restaurantId = restaurantManager.Restaurant.RestaurantId;
            var requests = _dbContext.Request.Where(p => p.RestaurantId == restaurantId)
                .SingleOrDefault(p => p.RestaurantId == id);
            return SupplyResponse.Ok(requests);
        }

        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse Post(int id, [FromBody] RequestRequest requestRequest)
        {
            var currentUser = HttpContext.User;
            var dbUser =
                _dbContext.User.Include(p => p.RestaurantManager).ThenInclude(p => p.Restaurant)
                    .SingleOrDefault(p => currentUser.FindFirst(ClaimTypes.Name).Value.Equals(p.UserName));
            if (dbUser == null) return SupplyResponse.Fail("Unauthorize", "Your are not the user in the system.");
            var restaurantManager = dbUser.RestaurantManager;
            if (restaurantManager == null)
                return SupplyResponse.Fail("Unauthorize", "Your are not the restaurant manager.");
            var restaurantId = restaurantManager.Restaurant.RestaurantId;

            var request = new Models.Request
            {
                RestaurantId = restaurantId,
                Creator = dbUser.UserId
            };

            request = _dbContext.Request.Add(request).Entity;

            foreach (var item in requestRequest.Items)
            {
                var virtualItem =
                    _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId.Equals(item.VirtualItemId));
                if (virtualItem == null) return SupplyResponse.NotFound("virtual item", item.VirtualItemId);
                var requestItem = new RequestItem
                {
                    RequestId = request.RequestId,
                    VirtualItemId = virtualItem.Id,
                    Quantity = item.Quantity
                };
                _dbContext.RequestItem.Add(requestItem);
            }

            _dbContext.SaveChanges();

            return Get(request.RequestId);
        }


        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id, [FromBody] IdRequest idRequest)
        {

            return SupplyResponse.Ok();
        }
    }
}