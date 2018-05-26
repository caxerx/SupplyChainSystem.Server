using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            /*
             HOW TO FIND USERNAME AND ROLE OF USER
            var currentUser = HttpContext.User;
            Console.WriteLine(currentUser.FindFirst(ClaimTypes.Name).Value);
            Console.WriteLine(currentUser.FindFirst(ClaimTypes.Role).Value);
            */
            var users = _dbContext.User.Select(p => p);
            foreach (var user in users) user.Password = null;
            return SupplyResponse.Ok(users);
        }

        // GET api/user/3
        [HttpGet("{id}")]
        [Authorize]
        public SupplyResponse Get(int id)
        {
            var user = _dbContext.User.SingleOrDefault(p => p.UserId == id);
            if (user == null) return SupplyResponse.NotFound("user", id + "");
            user.Password = null;
            return SupplyResponse.Ok(user);
        }

        // POST api/user
        //[HttpPost, Authorize(Roles = "Admin")]
        [HttpPost]
        [Authorize]
        public SupplyResponse Post([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.UserType) || string.IsNullOrWhiteSpace(user.Password))
                return SupplyResponse.RequiredFieldEmpty();
            var entity = _dbContext.User.AsNoTracking().SingleOrDefault(p => p.UserName == user.UserName);
            if (entity != null)
                return SupplyResponse.DuplicateEntry("user",user.UserName);
            user.UserId = 0;
            user.Password = HashUtilities.HashPassword(user.Password);
            _dbContext.User.Add(user);
            _dbContext.SaveChanges();
            return Get(user.UserId);
        }

        // PUT api/user/5
        [HttpPut("{id}")]
        [Authorize]
        public SupplyResponse Put(int id, [FromBody] User user)
        {
            var entity = _dbContext.User.AsNoTracking().SingleOrDefault(p => p.UserId == id);
            if (entity == null) return SupplyResponse.NotFound("user", id + "");
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.UserType))
                return SupplyResponse.BadRequest("Required Field is Empty");

            user.UserId = id;
            var oldPw = entity.Password;
            user.Password = string.IsNullOrWhiteSpace(user.Password)
                ? oldPw
                : HashUtilities.HashPassword(user.Password);

            _dbContext.Attach(user);
            _dbContext.Entry(user).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Get(id);
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        [Authorize]
        public SupplyResponse Delete(int id)
        {
            var user = _dbContext.User.SingleOrDefault(p => p.UserId == id);
            if (user == null) return SupplyResponse.NotFound("user", id + "");
            _dbContext.Remove(user);
            _dbContext.SaveChanges();
            return SupplyResponse.Ok();
        }
    }
}