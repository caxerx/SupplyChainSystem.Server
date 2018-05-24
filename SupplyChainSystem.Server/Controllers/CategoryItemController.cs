using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        [HttpGet, Authorize]
        public ActionResult Get()
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

                foreach (var cateI in cate.CategoryItems)
                {
                    cateR.VirtualItemId.Add(cateI.VirtualItemId);
                }

                cate.CategoryItems = null;
                cateRes.Add(cateR);
            }

            return Ok(SupplyResponse.Ok(cateRes));
        }

        // GET api/user/3
        [HttpGet("{id}"), Authorize]
        public ActionResult Get(int id)
        {
            var category = _dbContext.Category.Include(a => a.CategoryItems).ThenInclude(a => a.VirtualItem)
                .SingleOrDefault(p => p.CategoryId == id);
            if (category == null)
            {
                return Ok(SupplyResponse.NotFound());
            }

            var cateRes = new CategoryItemResponse
            {
                Category = category,
                VirtualItemId = new List<string>()
            };

            foreach (var cateI in category.CategoryItems)
            {
                cateRes.VirtualItemId.Add(cateI.VirtualItemId);
            }

            category.CategoryItems = null;

            return Ok(SupplyResponse.Ok(cateRes));
        }


        // POST api/category/{id}/add
        [HttpPost("{id}"), Authorize]
        public ActionResult AddToCategory(int id, [FromBody] IdRequest idRequest)
        {
            var category = _dbContext.Category.SingleOrDefault(p => p.CategoryId == id);

            var vItm = _dbContext.VirtualItem.SingleOrDefault(p => p.VirtualItemId == idRequest.Id);

            if (category == null || vItm == null)
            {
                return Ok(SupplyResponse.NotFound());
            }

            if (_dbContext.CategoryItem.SingleOrDefault(p =>
                    p.CategoryId == category.CategoryId && p.VirtualItemId == vItm.VirtualItemId) != null)
            {
                return Ok(SupplyResponse.Fail("Duplicate Entry"));
            }

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
        [HttpDelete("{id}"), Authorize]
        public ActionResult RemoveFromCategory(int id, [FromBody] IdRequest idRequest)
        {
            var cateItm =
                _dbContext.CategoryItem.SingleOrDefault(p => p.CategoryId == id && p.VirtualItemId == idRequest.Id);
            if (cateItm == null)
            {
                return BadRequest();
            }

            _dbContext.CategoryItem.Remove(cateItm);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}