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
    public class AgreementController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public AgreementController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var agreements = _dbContext.Agreement.Select(p => p);
            return SupplyResponse.Ok(agreements);
        }


        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var agreement = _dbContext.Agreement.SingleOrDefault(p => p.AgreementId == id);
            return SupplyResponse.Ok(agreement);
        }

        /*
        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse Post(int id, [FromBody] Agreement agreement)
        {

        }

        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] Agreement agreement)
        {
        }

        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id, [FromBody] Agreement agreement)
        {
        }
        */
    }
}