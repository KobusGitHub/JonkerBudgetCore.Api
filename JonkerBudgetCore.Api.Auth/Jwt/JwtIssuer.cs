using System;
using System.Security.Claims;
using JonkerBudgetCore.Api.Auth.User;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace JonkerBudgetCore.Api.Auth.Jwt
{
    public class JwtIssuer : IJwtIssuer
    {
        public async Task<string> IssueToken(ApplicationUser applicationUser,
            JwtIssuerOptions jwtOptions,
            ClaimsIdentity identity)
        {
            var claims = new List<Claim>
            {
                    new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, await jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                              ToUnixEpochDate(jwtOptions.IssuedAt).ToString(),
                              ClaimValueTypes.Integer64),
                    identity.FindFirst("uid"),
                    identity.FindFirst("firstName"),
                    identity.FindFirst("surname")
            };


            //Add on any roles
            claims.AddRange(identity.Claims.Where(s => s.Type == "roles"));
            
            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                notBefore: jwtOptions.NotBefore,
                expires: jwtOptions.Expiration,
                signingCredentials: jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
    }
}

