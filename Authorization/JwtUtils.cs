using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OntrackDb.Authentication;
using OntrackDb.Model;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OntrackDb.Authorization
{
    public class JwtUtils : IJwtUtils
    {
        
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public JwtUtils( IConfiguration configuration, UserManager<Authentication.User> userManager)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(LoginModel model)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null) // password validation is not requied, as this method is called after successful login/verification
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                return tokenHandler.WriteToken(token);
            }
            return null;
        }

        public async Task<string> GenerateTokenForVerification(string email , int duration)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var user = await _userManager.FindByEmailAsync(email);
           
            
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(duration),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                return tokenHandler.WriteToken(token);
            
        }


        public string ValidateToken(string token)
        {
                if (token == null)
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var userId = jwtToken.Claims.First(x => x.Type == "Id").Value;

                    // return user id from JWT token if validation successful
                    return userId;
                }
                catch
                {
                    // return null if validation fails
                    return null;
                }
         }

        public string GetRole(string token) {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var role = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                // return user id from JWT token if validation successful
                return role;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }



    }
}
