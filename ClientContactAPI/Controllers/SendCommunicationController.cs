using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;
using ClientContactAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ClientContactAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    [Authorize]
    public class SendCommunicationController : ControllerBase
    {
        private readonly ICommunication _communication;
        private readonly ITemplate _template;
        private readonly ICustomer _customer;
        public SendCommunicationController(ICommunication communication, ITemplate template, ICustomer customer)
        {
            _communication = communication;
            _template = template;
            _customer = customer;
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CommunicationSend(CommunicationViewModel communication)
        {
            APIResponse result;
            try
            {
                var customer = await _customer.GetCustomer(communication.CustomerId);
                if (customer == null)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Data = null,
                        Message = "Customer not found."
                    };
                    return new BadRequestObjectResult(result);
                }

                var template = await _template.GetTemplate(communication.TemplateId);
                if (template == null)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Data = null,
                        Message = "Template not found."
                    };
                    return new BadRequestObjectResult(result);
                }
                result = await _communication.SendCommunication(template, customer);
 
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
        [HttpPost("broadcast")]
        public async Task<ActionResult<APIResponse>> CommunicationSendToAll(CommunicationAllViewModel communication)
        {
            APIResponse result;
            try
            {
                var customers = await _customer.GetCustomers();
                if (customers?.Any() != true)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Data = null,
                        Message = "No customers found to send communication."
                    };
                    return new BadRequestObjectResult(result);
                }

                var template = await _template.GetTemplate(communication.TemplateId);
                if (template == null)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Data = null,
                        Message = "Template not found."
                    };
                    return new BadRequestObjectResult(result);
                }

                var failedList = new List<string>();
                foreach (var customer in customers)
                {
                    var response = await _communication.SendCommunication(template, customer);
                    if (!response.Success) failedList.Add(customer.Id);
                }

                if (failedList.Count > 0)
                {
                    result = new APIResponse()
                    {
                        Success = true,
                        Data = failedList,
                        Message = $"It was not possible to communicate {failedList.Count} customers.",
                    };
                }
                else
                {
                    result = new APIResponse()
                    {
                        Success = true,
                        Data = null,
                        Message = $"E-mail sent successfully to all customers.",
                    };
                }
                
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
