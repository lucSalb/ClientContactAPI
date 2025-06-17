using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;
using ClientContactAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utils = ClientContactAPI.Utils;

namespace ClientContactAPI.Controllers
{
    [ApiController] 
    [Route("/api/[controller]/")]
    [Authorize]
    public class CustomerManagementController : ControllerBase
    {
        private readonly ICustomer _customer;
        public CustomerManagementController(ICustomer customer)
        {
            _customer = customer;
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetCustomerList()
        {
            APIResponse result;
            try
            {
                result = new APIResponse()
                {
                    Success = true,
                    Message = "",
                    Data = await _customer.GetCustomers()
                };
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
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse>> GetCustomer(string id)
        {
            APIResponse result;
            try
            {
                var found = await _customer.GetCustomer(id);
                if (found == null)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Message = "Customer not found.",
                        Data = null
                    };
                    return NotFound(result);
                }

                result = new APIResponse()
                {
                    Success = true,
                    Message = "",
                    Data = found
                };

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
        [HttpPost]
        public async Task<ActionResult<APIResponse>> RegisterCustomer(CustomerViewModel customer)
        {
            APIResponse result;
            try
            {
                if (!ModelState.IsValid)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Message = "Please fill all the necessary fields.",
                        Data = null
                    };
                    return new BadRequestObjectResult(result);
                }

                if (!await Utils.ValidateEmail(customer.Email))
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Message = "Invalid e-mail address.",
                        Data = null
                    };
                    return new BadRequestObjectResult(result);
                }

                var newCustomer = new Customer()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = customer.Name,
                    Email = customer.Email,
                };
                result = await _customer.RegisterCustomer(newCustomer);
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
        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse>> UpdateCustomer(CustomerViewModel customer, string id)
        {
            APIResponse result;
            try
            {
                if (!ModelState.IsValid)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Message = "Please fill all the necessary fields.",
                        Data = null
                    };
                    return new BadRequestObjectResult(result);
                }

                if (!await Utils.ValidateEmail(customer.Email))
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Message = "Invalid e-mail address.",
                        Data = null
                    };
                    return new BadRequestObjectResult(result);
                }

                Customer updateCustomer = new Customer()
                {
                    Id = id,
                    Name = customer.Name,
                    Email = customer.Email, 
                };

                result = await _customer.UpdateCustumer(updateCustomer);
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
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse>> RemoveCustomer(string id)
        {
            APIResponse result;
            try
            {
                result = await _customer.DeleteCustomer(id);
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
