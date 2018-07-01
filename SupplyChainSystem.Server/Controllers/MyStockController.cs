using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Models;
using SupplyChainSystem.Server.ResponseWrapper;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MyStockController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public MyStockController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        [HttpGet]
        public SupplyResponse Get()
        {
            var currentUser = HttpContext.User;
            var dbUser =
                _dbContext.User.Include(p => p.RestaurantManager).ThenInclude(p => p.Restaurant)
                    .SingleOrDefault(p => currentUser.FindFirst(ClaimTypes.Name).Value.Equals(p.UserName));

            if (dbUser == null) return SupplyResponse.Fail("Unauthorize", "Your are not the user in the system.");

            if (dbUser.UserType == UserType.RestaurantManager)
            {
                var restaurantManager = dbUser.RestaurantManager;
                if (restaurantManager == null)
                    return SupplyResponse.Fail("Unauthorize", "Your are not the restaurant manager of any restaurant.");
                return SupplyResponse.Ok(new {stock = restaurantManager.Restaurant.StockId});
            }

            var id = _dbContext.Stock.FirstOrDefault(p => p.StockType == StockType.WarehouseStock).StockId;
            return SupplyResponse.Ok(new {stock = id});
        }
    }
}