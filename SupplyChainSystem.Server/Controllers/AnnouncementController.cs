using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SupplyChainSystem.Server.Hub;
using SupplyChainSystem.Server.Models;
using SupplyChainSystem.Server.ResponseWrapper;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AnnouncementController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        private readonly IHubContext<NotificationHub> _hubContext;

        public AnnouncementController(ProcedurementContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }


        [Authorize]
        [HttpGet]
        public SupplyResponse Get()
        {
            var datas = _dbContext.DataCache.Where(p => p.CacheType == "Announcement")
                .Select(p => JsonConvert.DeserializeObject(p.Content));
            return SupplyResponse.Ok(datas);
        }


        [Authorize]
        [HttpPost]
        public SupplyResponse Post([FromBody] Announcement data)
        {
            _dbContext.DataCache.Add(new DataCache
            {
                CacheType = "Announcement",
                CacheTime = DateTime.Now,
                Content = JsonConvert.SerializeObject(data),
                RemovalTime = data.RemovalTime
            });
            _dbContext.SaveChanges();
            _hubContext.Clients.All.SendAsync("ReceiveMessage", "All", "A new announcement broadcasted");
            return Get();
        }
    }
}