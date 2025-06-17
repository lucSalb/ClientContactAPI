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
    public class TemplateManagementController : ControllerBase
    {
        private readonly ITemplate _template;
        public TemplateManagementController(ITemplate template) 
        { 
            _template = template;
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetTemplateList()
        {
            APIResponse result;
            try
            {
                result = new APIResponse()
                {
                    Success = true,
                    Message = "",
                    Data = await _template.GetTemplates()
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
        public async Task<ActionResult<APIResponse>> GetTemplate(string id)
        {
            APIResponse result;
            try
            {
                var found = await _template.GetTemplate(id);
                if (found == null)
                {
                    result = new APIResponse()
                    {
                        Success = false,
                        Message = "Template not found.",
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
        public async Task<ActionResult<APIResponse>> RegisterTemplate(TemplateViewModel template)
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
                 
                var newTemplate = new Template()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = template.Name,
                    Subject = template.Subject,
                    Body = template.Body,
                };
                result = await _template.RegisterTemplate(newTemplate);
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
        public async Task<ActionResult<APIResponse>> UpdateTemplate(TemplateViewModel template, string id)
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

                Template updateTemplate = new Template()
                {
                    Id = id,
                    Name = template.Name,
                    Subject = template.Subject,
                    Body = template.Body,
                };

                result = await _template.UpdateTemplate(updateTemplate);
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
        public async Task<ActionResult<APIResponse>> RemoveTemplate(string id)
        {
            APIResponse result;
            try
            {
                result = await _template.DeleteTemplate(id);
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
