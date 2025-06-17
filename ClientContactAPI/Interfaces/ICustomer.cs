using ClientContactAPI.Classes;
using System.Net;

namespace ClientContactAPI.Interfaces
{
    public interface ICustomer
    {
        public Task<bool> CreateCustomerDB();
        public Task<List<Customer>> GetCustomers();
        public Task<Customer?> GetCustomer(string id);
        public Task<APIResponse> RegisterCustomer(Customer customer);
        public Task<APIResponse> UpdateCustumer(Customer customer);
        public Task<APIResponse> DeleteCustomer(string id);
    }
}
