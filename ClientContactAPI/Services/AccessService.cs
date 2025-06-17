using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;
using ClientContactAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClientContactAPI.Services
{
    public class AccessService : IAccess
    {
        private static IConfiguration _configuration;
        public AccessService(IConfiguration configuration)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();
        }
        public APIResponse Login(LoginViewModel model)
        {
            APIResponse result;
            var t = Utils.HashGenerator("Password123").ToUpper();
            if (model.Username == "admin".ToUpper() && model.HashPassword.ToUpper() == Utils.HashGenerator("Password123").ToUpper())
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(issuer: "ClientContactAPI",
                                                 audience: "ClientContactAPIUser",
                                                 claims: claims,
                                                 expires: DateTime.Now.AddHours(1),
                                                 signingCredentials: creds);

                result = new APIResponse() { Success = true, Data = new JwtSecurityTokenHandler().WriteToken(token), Message = "Login successful." };
            }
            else
            {
                result = new APIResponse() { Success = false, Data = null, Message = "Username/password is incorrect." };
            }
            return result;
        }
    }
}
