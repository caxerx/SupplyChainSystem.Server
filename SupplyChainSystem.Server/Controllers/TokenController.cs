using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IConfiguration _config;
        private readonly ProcedurementContext _dbContext;

        public TokenController(ProcedurementContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody] LoginRequest login)
        {
            IActionResult response = Ok(SupplyResponse.Fail("Unauthorized","User not found"));
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(SupplyResponse.Ok(new {token = tokenString}));
            }

            return response;
        }

        [HttpGet]
        [Authorize]
        public IActionResult TestToken()
        {
            return Ok(SupplyResponse.Ok());
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
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds,
                claims: claims
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(LoginRequest login)
        {
            var user =
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