using ClientContactAPI.Classes;
using System.Net;

namespace ClientContactAPI.Interfaces
{
    public interface ICustomer
    {
        public List<Customer> GetCustomers();
        public Customer GetCustomerById(string id);
        public OperationResult RegisterCustomer(Customer customer);
        public OperationResult UpdateCustumer(Customer customer);
        public OperationResult DeleteCustomer(string id);
    }
}
