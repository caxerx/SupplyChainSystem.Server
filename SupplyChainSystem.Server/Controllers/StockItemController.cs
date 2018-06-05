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
    public class StockItemController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public StockItemController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/user
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var stocks = _dbContext.Stock.Include(a => a.StockItem).ThenInclude(a => a.VirtualItem).Select(p => p);
            return SupplyResponse.Ok(stocks);
        }

        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var stock = _dbContext.Stock.Include(a => a.StockItem).ThenInclude(a => a.VirtualItem)
                .SingleOrDefault(p => p.StockId == id);
            return SupplyResponse.Ok(stock);
        }

        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse AddStock(int id, [FromBody] ItemRequest itemRequest)
        {
            var stock = _dbContext.Stock.SingleOrDefault(p => p.StockId == id);
            var vItm = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == itemRequest.VirtualItemId);

            if (stock == null) return SupplyResponse.NotFound("stock", id + "");
            if (vItm == null) return SupplyResponse.NotFound("virtual item", itemRequest.VirtualItemId + "");

            var stockItem = _dbContext.StockItem.SingleOrDefault(p =>
                p.StockId == id && p.VirtualItem.VirtualItemId == vItm.VirtualItemId);

            if (stockItem != null)
            {
                stockItem.Quantity += itemRequest.Quantity;
            }
            else
            {
                stockItem = new StockItem
                {
                    StockId = id,
                    VirtualItemId = vItm.Id,
                    Quantity = itemRequest.Quantity
                };
                _dbContext.StockItem.Add(stockItem);
            }

            _dbContext.SaveChanges();

            return Get(stock.StockId);
        }

        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse SetStock(int id, [FromBody] ItemRequest itemRequest)
        {
            var stock = _dbContext.Stock.SingleOrDefault(p => p.StockId == id);
            var vItm = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == itemRequest.VirtualItemId);

            if (stock == null) return SupplyResponse.NotFound("stock", id + "");
            if (vItm == null) return SupplyResponse.NotFound("virtual item", itemRequest.VirtualItemId + "");

            var stockItem = _dbContext.StockItem.SingleOrDefault(p =>
                p.StockId == id && p.VirtualItem.VirtualItemId == vItm.VirtualItemId);

            if (stockItem != null)
            {
                stockItem.Quantity = itemRequest.Quantity;
            }
            else
            {
                stockItem = new StockItem
                {
                    StockId = id,
                    VirtualItemId = vItm.Id,
                    Quantity = itemRequest.Quantity
                };
                _dbContext.StockItem.Add(stockItem);
            }

            _dbContext.SaveChanges();

            return Get(stock.StockId);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse RemoveFromStock(int id, [FromBody] IdRequest idRequest)
        {
            var stockItem =
                _dbContext.StockItem.SingleOrDefault(p =>
                    p.StockId == id && p.VirtualItem.VirtualItemId == idRequest.Id);
            if (stockItem == null) return SupplyResponse.NotFound("stock item", $"\"{idRequest.Id} in {id}\"");

            _dbContext.StockItem.Remove(stockItem);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}