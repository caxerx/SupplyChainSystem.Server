using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly ProcedurementContext _dbContext;
        private readonly IConfiguration _config;

        public TokenController(ProcedurementContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody] LoginRequest login)
        {
            IActionResult response = Ok(SupplyResponse.Fail("Unauthorized"));
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(SupplyResponse.Ok(new { token = tokenString }));
            }

            return response;
        }


        private string BuildToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.UserType)
            };
            var token = new JwtSecurityToken(issuer:_config["Jwt:Issuer"],
                audience:_config["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds,
                claims:claims
                );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(LoginRequest login)
        {
            User user =
                _dbContext.User.SingleOrDefault(u =>
                    u.UserName == login.Username && u.Password == HashUtilities.HashPassword(login.Password));

            return user;
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}