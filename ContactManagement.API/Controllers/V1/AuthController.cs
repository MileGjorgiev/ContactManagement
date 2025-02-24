using ContactManagement.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContactManagement.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            
            if (loginDto.Username != "login" || loginDto.Password != "login")
            {
                return Unauthorized("Invalid credentials");
            }

           
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, loginDto.Username),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("eZkz8l3XYuV8Zt9tRpw89nA9DbYy5PZ5TrJ1mTgK1yU="));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ContactManagement",
                audience: "ContactManagement",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            
            return Ok(new { token = tokenString });
        }
    }
}
