using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ClientContactAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class BaseAPIController : Controller
    {
        private readonly ICustomer _customer;
        private readonly ITemplate _template;
        public BaseAPIController(ICustomer customer, ITemplate template)
        {
            _customer = customer;
            _template = template;
        }
        [HttpGet("create")]
        public async Task<IActionResult> CreateDatabase()
        {
            if (!await _customer.CreateCustomerDB()) return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse() { Success = false, Message = "Something went wrong while creating CustomersTable", Data = null }) ;
            if (!await _template.CreateTemplateDB()) return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse() { Success = false, Message = "Something went wrong while creating TemplatesTable", Data = null });

            return new OkObjectResult(new APIResponse() { Success = true, Message = "Database`s tables created." , Data = null });
        }
    }
}
