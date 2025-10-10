using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CityHubCore.Application.Jwt {
    public class JwtHelper {
        static public string GenerateJwtToken(JwtConfig jwtConfig, JwtUserInfo jwtUserInfo) {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Issuer = jwtConfig.Issuer,
                Audience = jwtConfig.Audience,
                Subject = new ClaimsIdentity(new[] {
                    new Claim("CompanyId", jwtUserInfo.CompanyId )
                    , new Claim("UserId", jwtUserInfo.UserId )
                    , new Claim("SID", jwtUserInfo.SID )
                }),
                Expires = DateTime.UtcNow.AddMinutes(jwtConfig.AccessTokenExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        static public string GenerateRefreshToken(JwtConfig jwtConfig, JwtUserInfo jwtUserInfo, string ipAddress) {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(jwtConfig.RefreshTokenSecret);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Issuer = jwtConfig.Issuer,
                Audience = jwtConfig.Audience,
                Subject = new ClaimsIdentity(new[] {
                    new Claim("CompanyId", jwtUserInfo.CompanyId )
                    , new Claim("UserId", jwtUserInfo.UserId )
                    , new Claim("SID", jwtUserInfo.SID )
                    , new Claim("IP", ipAddress )
                }),
                Expires = DateTime.UtcNow.AddHours(jwtConfig.RefreshTokenExpiresInHours),
                //Expires = DateTime.UtcNow.AddMinutes(jwtConfig.RefreshTokenExpiresInHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        static public JwtUserInfo ValidateJwtToken(string issuer, string aduience, string secret, string token, bool validateLifetime = true) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            tokenHandler.ValidateToken(FormatToken(token), new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = aduience,

                ValidateLifetime = validateLifetime,
                RequireExpirationTime = true,

                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            JwtUserInfo userJWTInfo = new JwtUserInfo() {
                CompanyId = jwtToken.Claims.First(x => x.Type == "CompanyId").Value,
                UserId = jwtToken.Claims.First(x => x.Type == "UserId").Value,
                SID = jwtToken.Claims.First(x => x.Type == "SID").Value
            };

            return userJWTInfo;
        }

        static public string FormatToken(string token) {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token), "token is null");

            return token.Replace("\"", "");
        }
    }
}
