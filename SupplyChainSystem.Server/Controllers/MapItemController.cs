using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MapItemController : Controller
    {

        private readonly ProcedurementContext _dbContext;

        public MapItemController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/MapItem
        [Authorize]
        [HttpGet]
        public ActionResult Get(string realItemId,string virtualItemId)
        {
            _dbContext.VirtualIdMap.Add(new VirtualIdMap(realItemId, virtualItemId));
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
