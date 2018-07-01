using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                .Select(p => new
                {
                    CreateTime = p.CacheTime,
                    Id = p.DataCacheId,
                    Data = JsonConvert.DeserializeObject(p.Content)
                });
            return SupplyResponse.Ok(datas);
        }


        [Authorize]
        [HttpDelete("{id}")]
        public SupplyResponse Delete(int id)
        {
            var datas = _dbContext.DataCache.SingleOrDefault(p => p.DataCacheId == id);
            _dbContext.DataCache.Remove(datas);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }


        [Authorize]
        [HttpPost]
        public SupplyResponse Post([FromBody] Announcement data)
        {
            if (string.IsNullOrWhiteSpace(data.Message) || string.IsNullOrWhiteSpace(data.Title) ||
                data.Target == null || !data.Target.Any())
            {
                return SupplyResponse.RequiredFieldEmpty();
            }

            if (data.RemovalTime < DateTime.Now)
            {
                return SupplyResponse.BadRequest("Removal Time should be a future date");
            }

            _dbContext.DataCache.Add(new DataCache
            {
                CacheType = "Announcement",
                CacheTime = DateTime.Now,
                Content = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
                RemovalTime = data.RemovalTime
            });
            _dbContext.SaveChanges();
            _hubContext.Clients.All.SendAsync("ReceiveAnnouncement", data.Target, "A new announcement broadcasted");
            return Get();
        }
    }
}