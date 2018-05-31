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
    public class CategoryItemController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public CategoryItemController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/user
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var categories = _dbContext.Category.Include(a => a.CategoryItems).ThenInclude(a=>a.VirtualItem).Select(p => p);

            var categoryItemResponses = new List<CategoryItemResponse>();
            foreach (var category in categories)
            {
                var categoryItemResponse = new CategoryItemResponse
                {
                    Category = category,
                    VirtualItemId = new List<string>()
                };

                if (category.CategoryItems != null)
                {
                    foreach (var categoryItem in category.CategoryItems)
                        categoryItemResponse.VirtualItemId.Add(categoryItem.VirtualItem.VirtualItemId);
                    category.CategoryItems = null;
                }

                categoryItemResponses.Add(categoryItemResponse);
            }

            return SupplyResponse.Ok(categoryItemResponses);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var category = _dbContext.Category.Include(a => a.CategoryItems).ThenInclude(a => a.VirtualItem).SingleOrDefault(p => p.CategoryId == id);

            if (category == null) return SupplyResponse.NotFound("category", id + "");


            var categoryItemResponse = new CategoryItemResponse
            {
                Category = category,
                VirtualItemId = new List<string>()
            };

            if (category.CategoryItems != null)
            {
                foreach (var categoryItem in category.CategoryItems)
                    categoryItemResponse.VirtualItemId.Add(categoryItem.VirtualItem.VirtualItemId);
                category.CategoryItems = null;
            }


            return SupplyResponse.Ok(categoryItemResponse);
        }


        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse AddToCategory(int id, [FromBody] IdRequest idRequest)
        {
            var category = _dbContext.Category.SingleOrDefault(p => p.CategoryId == id);

            var vItm = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == idRequest.Id);

            if (category == null) return SupplyResponse.NotFound("category", id + "");
            if (vItm == null) return SupplyResponse.NotFound("virtual item", idRequest.Id + "");

            var cItem = _dbContext.CategoryItem.SingleOrDefault(p =>
                p.CategoryId == category.CategoryId && p.VirtualItem.VirtualItemId == vItm.VirtualItemId);
            if (cItem != null)
                return SupplyResponse.DuplicateEntry("category item",
                    $"\"{cItem.VirtualItemId} in {cItem.CategoryId}\"");

            var cateItm = new CategoryItem
            {
                CategoryId = category.CategoryId,
                VirtualItemId = vItm.Id
            };

            _dbContext.CategoryItem.Add(cateItm);
            _dbContext.SaveChanges();
            return Get(category.CategoryId);
        }

        // POST api/category/{id}/add
        [
            HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse RemoveFromCategory(int id, [FromBody] IdRequest idRequest)
        {
            var cItem =
                _dbContext.CategoryItem.SingleOrDefault(p =>
                    p.CategoryId == id && p.VirtualItem.VirtualItemId == idRequest.Id);
            if (cItem == null) return SupplyResponse.NotFound("category item", $"\"{idRequest.Id} in {id}\"");

            _dbContext.CategoryItem.Remove(cItem);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}