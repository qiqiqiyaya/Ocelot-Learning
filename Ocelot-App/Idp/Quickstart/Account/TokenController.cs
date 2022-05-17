using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Idp.Quickstart.Account
{
    [AllowAnonymous]
    public class TokenController : Controller
    {
        public string GenerateToken()
        {
            var SymmetricKey = "YQBiAGMAZABlAGYAZwBoAGkAagBrAGwAbQBuAG8AcABxAHIAcwB0AHUAdgB3AHgAeQB6ADAAMQAyADMANAA1AA==";
            var key = new SymmetricSecurityKey(Convert.FromBase64String(SymmetricKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var expirationDate = DateTime.UtcNow.AddHours(2);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, "fdsafdsa"),
                new Claim("Role", "Admin"),
                new Claim("aut", "http://localhost:5002"),
            };

            var token = new JwtSecurityToken(audience: "apiAudience",
                issuer: "http://localhost:5002",
                claims: claims,
                expires: expirationDate,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
