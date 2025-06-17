using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClientContactAPI.Services
{
    public class CommunicationService : ICommunication
    {
        public async Task<APIResponse> SendCommunication(Template template, Customer customer)
        {
            template.Body = template.Body.Replace("[NAME]", Utils.PrettyTextDisplay(customer.Name));
            template.Body = template.Body.Replace("[EMAIL]", Utils.PrettyTextDisplay(customer.Email));

            Console.Write(template.Subject + "\r\n" + template.Body);

            return new APIResponse() { Success = true, Data = null, Message = $"E-mail sent to {customer.Name}" };
        } 
    }
}
