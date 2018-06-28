using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Server.Hub;
using SupplyChainSystem.Server.Models;
using SupplyChainSystem.Server.ResponseWrapper;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly ProcedurementContext _dbContext;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TestController(ProcedurementContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }


        [Microsoft.AspNetCore.Authorization.Authorize]
        public SupplyResponse Get()
        {
            _hubContext.Clients.All.SendAsync("ReceiveMessage", "r", "rrr");
            return SupplyResponse.Ok();
        }
    }
}