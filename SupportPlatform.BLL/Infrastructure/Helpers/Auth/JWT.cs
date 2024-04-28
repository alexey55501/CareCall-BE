using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SupportPlatform.BLL.Infrastructure.Helpers.Auth
{
    public static class JWT
    {
        public static JwtSecurityToken GenerateToken(
            IConfiguration configuration,
            List<Claim> authClaims, DateTime? expireDate = null) =>
                new JwtSecurityToken(
                            issuer: configuration["JWT:ValidIssuer"],
                            audience: configuration["JWT:ValidAudience"],
                            expires: expireDate ?? DateTime.Now.AddMonths(3),
                            claims: authClaims,
                            signingCredentials: new SigningCredentials(
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                                SecurityAlgorithms.HmacSha256)
                        );
    }
}
