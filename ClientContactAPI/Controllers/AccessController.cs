using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;
using ClientContactAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Utils = ClientContactAPI.Utils;

namespace ClientContactAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : ControllerBase
    {
        private readonly IAccess _access;
        public AccessController(IAccess access)
        {
            _access = access;
        }
        [HttpPost("Login")]
        public IActionResult Login(LoginViewModel model)
        {
            APIResponse result;
            try
            {
                if (!ModelState.IsValid)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Data = null,
                        Message = "Please fill all the informations."
                    };
                    return new BadRequestObjectResult(result);
                }
                result = _access.Login(model); 
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                result = new APIResponse()
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return StatusCode(Convert.ToInt16(HttpStatusCode.InternalServerError), result);
            }
        }
    }
}
