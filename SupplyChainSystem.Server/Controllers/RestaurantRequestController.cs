using System;
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
    public class RestaurantRequestController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public RestaurantRequestController(ProcedurementContext dbContext)
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
            var requests = _dbContext.Request.Include(p => p.User).Include(p => p.RequestItem)
                .Where(p => p.RestaurantId == restaurantId)
                .Select(p => p);
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
            var requests = _dbContext.Request.Include(p => p.User).Include(p => p.RequestItem)
                .Where(p => p.RestaurantId == restaurantId)
                .SingleOrDefault(p => p.RequestId == id);
            if (requests == null) return SupplyResponse.NotFound("request", id + "");
            return SupplyResponse.Ok(requests);
        }

        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] ICollection<ItemRequest> itemRequest)
        {
            var currentUser = HttpContext.User;
            var dbUser =
                _dbContext.User.Include(p => p.RestaurantManager).ThenInclude(p => p.Restaurant)
                    .SingleOrDefault(p => currentUser.FindFirst(ClaimTypes.Name).Value.Equals(p.UserName));
            if (dbUser == null) return SupplyResponse.Fail("Unauthorize", "Your are not the user in the system.");
            var restaurantManager = dbUser.RestaurantManager;
            if (restaurantManager == null)
                return SupplyResponse.Fail("Unauthorize", "Your are not the restaurant manager.");

            var itemMap = new Dictionary<int, int>();
            var itemList = new List<RequestItem>();

            foreach (var item in itemRequest)
            {
                var virtualItem =
                    _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId.Equals(item.VirtualItemId));
                if (virtualItem == null) return SupplyResponse.NotFound("virtual item", item.VirtualItemId);
                if (itemMap.ContainsKey(virtualItem.Id))
                {
                    itemMap[virtualItem.Id] += item.Quantity;
                }
                else
                {
                    itemMap.Add(virtualItem.Id, item.Quantity);
                }
            }

            foreach (var (itemId, qty) in itemMap)
            {
                var requestItem = new RequestItem
                {
                    VirtualItemId = itemId,
                    Quantity = qty
                };
                itemList.Add(requestItem);
            }

            var restaurantId = restaurantManager.RestaurantId;
            var userId = dbUser.UserId;

            var request = new Models.Request
            {
                RestaurantId = restaurantId,
                RequestCreator = userId
            };

            _dbContext.Request.Add(request);
            _dbContext.SaveChanges();

            var requestId = request.RequestId;
            _dbContext.Entry(request).State = EntityState.Detached;


            foreach (var item in itemList)
            {
                item.RequestId = requestId;
                _dbContext.RequestItem.Add(item);
                _dbContext.SaveChanges();
                _dbContext.Entry(item).State = EntityState.Detached;
            }

            return Get(request.RequestId);
        }


        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] ICollection<ItemRequest> itemRequest)
        {
            var currentUser = HttpContext.User;
            var dbUser =
                _dbContext.User.Include(p => p.RestaurantManager).ThenInclude(p => p.Restaurant)
                    .SingleOrDefault(p => currentUser.FindFirst(ClaimTypes.Name).Value.Equals(p.UserName));
            if (dbUser == null) return SupplyResponse.Fail("Unauthorize", "Your are not the user in the system.");
            var restaurantManager = dbUser.RestaurantManager;
            if (restaurantManager == null)
                return SupplyResponse.Fail("Unauthorize", "Your are not the restaurant manager.");

            var request = _dbContext.Request.Include(p => p.RequestItem).SingleOrDefault(p => p.RequestId == id);
            if (request == null)
                return SupplyResponse.NotFound("Request", id + "");

            if (dbUser.UserId != request.RequestCreator)
                return SupplyResponse.BadRequest("You are not the creator of the request");

            ICollection<RequestItem> requestItems;
            if ((requestItems = request.RequestItem) != null)
            {
                foreach (var items in requestItems)
                {
                    _dbContext.Remove(items);
                }
            }

            _dbContext.SaveChanges();

            var itemMap = new Dictionary<int, int>();

            foreach (var item in itemRequest)
            {
                var virtualItem =
                    _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId.Equals(item.VirtualItemId));
                if (virtualItem == null) return SupplyResponse.NotFound("virtual item", item.VirtualItemId);
                if (itemMap.ContainsKey(virtualItem.Id))
                {
                    itemMap[virtualItem.Id] += item.Quantity;
                }
                else
                {
                    itemMap.Add(virtualItem.Id, item.Quantity);
                }
            }

            foreach (var (itemId, qty) in itemMap)
            {
                var requestItem = new RequestItem
                {
                    RequestId = request.RequestId,
                    VirtualItemId = itemId,
                    Quantity = qty
                };
                _dbContext.RequestItem.Add(requestItem);
                _dbContext.SaveChanges();
            }
            
            return Get(request.RequestId);
        }
    }
}