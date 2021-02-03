using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MainService.MiddleTier
{
    public class Authentication : IAuthentication
    {
        private IConfiguration configuration;
        private byte[] key;

        public Authentication(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWTSecret"));
        }

        public string GenerateJwtTokenForUser(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userid", user.UserId),
                    new Claim("username", user.UserName),
                    new Claim("rootfolderid", user.RootFolderId),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
