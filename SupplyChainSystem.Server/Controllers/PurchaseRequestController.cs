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
    public class PurchaseRequestController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public PurchaseRequestController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        public SupplyResponse Get()
        {
            var requests = _dbContext.Request.Include(p => p.User).Include(p => p.RequestItem).Select(p => p);
            return SupplyResponse.Ok(requests);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var requests = _dbContext.Request.Include(p => p.User).Include(p => p.RequestItem)
                .SingleOrDefault(p => p.RequestId == id);
            if (requests == null) return SupplyResponse.NotFound("request", id + "");
            return SupplyResponse.Ok(requests);
        }


        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id)
        {
            var request = _dbContext.Request.SingleOrDefault(p => p.RequestId == id);
            if (request == null) SupplyResponse.NotFound("Request", id + "");
            _dbContext.Remove(request);
            return SupplyResponse.Ok();
        }


        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] ICollection<ItemRequest> itemRequest)
        {
            var request = _dbContext.Request.Include(p => p.RequestItem).SingleOrDefault(p => p.RequestId == id);
            if (request == null)
                return SupplyResponse.NotFound("Request", id + "");

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