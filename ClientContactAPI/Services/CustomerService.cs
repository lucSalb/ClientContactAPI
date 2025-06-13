using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;

namespace ClientContactAPI.Services
{
    public class CustomerService : ICustomer
    {
        public List<Customer> GetCustomers()
        {
            throw new NotImplementedException();
        }

        public Customer GetCustomerById(string id)
        {
            throw new NotImplementedException();
        }

        public OperationResult RegisterCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }

        public OperationResult UpdateCustumer(Customer customer)
        {
            throw new NotImplementedException();
        }

        public OperationResult DeleteCustomer(string id)
        {
            throw new NotImplementedException();
        }
 
    }
}
