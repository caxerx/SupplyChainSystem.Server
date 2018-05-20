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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ProcedurementContext _dbContext;

        public UserController(ProcedurementContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/user
        [HttpGet, Authorize(Roles = "Admin")]
        public ActionResult Get()
        {
            /*
             HOW TO FIND USERNAME AND ROLE OF USER
            var currentUser = HttpContext.User;
            Console.WriteLine(currentUser.FindFirst(ClaimTypes.Name).Value);
            Console.WriteLine(currentUser.FindFirst(ClaimTypes.Role).Value);
            */

            var users = _dbContext.Users.Select(p => p);
            foreach (var user in users)
            {
                user.UserPassword = null;
            }

            return Ok(users);
        }

        // GET api/user/3
        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        public ActionResult Get(int id)
        {
            var user = _dbContext.Users.SingleOrDefault(p => p.UserId == id);
            if (user == null) return NoContent();
            user.UserPassword = null;
            return Ok(user);
        }

        // POST api/user
        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult Post([FromBody] User user)
        {
            user.UserPassword = HashUtilities.HashPassword(user.UserPassword);
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return Get(user.UserId);
        }

        // PUT api/user/5
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public ActionResult Put(int id, [FromBody] User user)
        {
            var entity = _dbContext.Users.AsNoTracking().SingleOrDefault(p => p.UserId == id);
            if (entity == null) return BadRequest();

            user.UserId = id;
            user.UserPassword = HashUtilities.HashPassword(user.UserPassword);

            _dbContext.Attach(user);
            _dbContext.Entry(user).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/user/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var entity = _dbContext.Users.SingleOrDefault(p => p.UserId == id);
            if (entity == null) return BadRequest();
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}