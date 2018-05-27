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
            var category = _dbContext.Category.Include(a => a.CategoryItems).Select(p => p);
            var cateRes = new List<CategoryItemResponse>();
            foreach (var cate in category)
            {
                var cateR = new CategoryItemResponse
                {
                    Category = cate,
                    VirtualItemId = new List<string>()
                };

                foreach (var cateI in cate.CategoryItems) cateR.VirtualItemId.Add(cateI.VirtualItemId);

                cate.CategoryItems = null;
                cateRes.Add(cateR);
            }

            return SupplyResponse.Ok(cateRes);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var category = _dbContext.Category.Include(a => a.CategoryItems).ThenInclude(a => a.VirtualItem)
                .SingleOrDefault(p => p.CategoryId == id);
            if (category == null) return SupplyResponse.NotFound("category", id + "");

            var cateRes = new CategoryItemResponse
            {
                Category = category,
                VirtualItemId = new List<string>()
            };

            foreach (var cateI in category.CategoryItems) cateRes.VirtualItemId.Add(cateI.VirtualItemId);

            category.CategoryItems = null;

            return SupplyResponse.Ok(cateRes);
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
                p.CategoryId == category.CategoryId && p.VirtualItemId == vItm.VirtualItemId);
            if (cItem != null)
                return SupplyResponse.DuplicateEntry("category item",
                    $"\"{cItem.VirtualItemId} in {cItem.CategoryId}\"");

            var cateItm = new CategoryItem
            {
                CategoryId = category.CategoryId,
                VirtualItemId = vItm.VirtualItemId
            };

            _dbContext.CategoryItem.Add(cateItm);
            _dbContext.SaveChanges();
            return Get(category.CategoryId);
        }

        // POST api/category/{id}/add
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse RemoveFromCategory(int id, [FromBody] IdRequest idRequest)
        {
            var cItem =
                _dbContext.CategoryItem.SingleOrDefault(p => p.CategoryId == id && p.VirtualItemId == idRequest.Id);
            if (cItem == null) return SupplyResponse.NotFound("category item", $"\"{idRequest.Id} in {id}\"");

            _dbContext.CategoryItem.Remove(cItem);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}