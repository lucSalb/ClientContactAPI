using ClientContactAPI.Classes;
using Microsoft.AspNetCore.Mvc;

namespace ClientContactAPI.Interfaces
{
    public interface ICommunication
    {
        public Task<APIResponse> SendCommunication(Template template, Customer customer);
    }
}
