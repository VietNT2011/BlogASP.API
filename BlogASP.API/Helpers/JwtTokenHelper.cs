using BlogASP.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogASP.API.Helpers
{
    public static class JwtTokenHelper
    {
        public static string GenerateJwtToken(IConfiguration configuration, User user)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //var claims = new[]
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub, userId),
            //    new Claim(JwtRegisteredClaimNames.UniqueName, username),
            //    new Claim(ClaimTypes.NameIdentifier, userId),
            //};
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId)
            };
            if (user.Role!.Count()>0)
            {
                foreach(var role in user.Role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                    claims.Add(new Claim("Role", role.RoleName));
                }
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
