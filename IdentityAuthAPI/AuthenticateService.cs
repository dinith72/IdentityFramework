using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityAuthAPI
{
    public class AuthenticateService
    {
        //AuthConfig _authConfig;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configeration;
        public AuthenticateService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            //  _authConfig = authConfig;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public string AuthenticateUser(IdentityUser user)
        {
            var utcNow = DateTime.UtcNow;
            var userRoles = _userManager.GetRolesAsync(user).Result;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , user.Id),
                 new Claim(ClaimTypes.Name, user.Id.ToString()),
                 new Claim(ClaimTypes.Role , userRoles[0] ),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),

                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
            };


            var secretBytes = Encoding.UTF8.GetBytes(_configeration.GetSection("Auth").GetSection("TokenKey").Value);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
               "https://localhost:44367",
               "https://localhost:44367",
                claims,
                notBefore: utcNow,
                expires: utcNow.AddHours(1),
                signingCredentials);

            string tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenJson;
        }
    }
}
