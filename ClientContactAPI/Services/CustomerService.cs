using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;
using Microsoft.Data.SqlClient;
using Utils = ClientContactAPI.Utils;

namespace ClientContactAPI.Services
{
    public class CustomerService : ICustomer
    {
        private static IConfiguration _configuration;
        static CustomerService()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();
        }
        public async Task<bool> CreateCustomerDB()
        {
            var connString = _configuration.GetConnectionString("connString");
            string queryString = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CustomersTABLE' )
                                    BEGIN
                                        CREATE TABLE [dbo].[CustomersTABLE](
                                            [Id]    NVARCHAR (50) NOT NULL,
                                            [Name]  NVARCHAR (80) NOT NULL,
                                            [Email] NVARCHAR (150) NOT NULL,
                                        )
                                    END";

            using(SqlConnection connection = new SqlConnection(connString))
            {
                await connection.OpenAsync();
                using(SqlCommand command = new SqlCommand(queryString, connection))
                {
                    await command.ExecuteNonQueryAsync(); 
                }
            }
            return true;
        }
        public async Task<List<Customer>> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();

            var connString = _configuration.GetConnectionString("connString");
            string sqlQuery = @"SELECT * FROM CustomersTABLE;";

            using (SqlConnection connection = new SqlConnection(connString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            customers.Add(new Customer()
                            {
                                Id = Utils.PrettyTextDisplay(reader["Id"].ToString()),
                                Name = Utils.PrettyTextDisplay(reader["Name"].ToString()),
                                Email = Utils.PrettyTextDisplay(reader["Email"].ToString())//
                            });
                        }
                    }
                }
            }

            return customers;
        }
        public async Task<Customer?> GetCustomer(string id)
        {
            Customer? customer = null;

            var sqlQuery = @"SELECT * FROM CustomersTABLE WHERE Id = UPPER(@Id);";
            var connString = _configuration.GetConnectionString("connString");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customer = new Customer()
                            {
                                Id = Utils.PrettyTextDisplay(reader["Id"].ToString()),
                                Name = Utils.PrettyTextDisplay(reader["Name"].ToString()),
                                Email = Utils.PrettyTextDisplay(reader["Email"].ToString())
                            };
                        }
                    }
                }
            }

            return customer;
        }
        public async Task<APIResponse> RegisterCustomer(Customer customer)
        {
            var sqlConnection = _configuration.GetConnectionString("connString");
            var countQuery = @"SELECT COUNT(*) FROM CustomersTABLE WHERE Name = UPPER(@Name) AND Email = UPPER(@Email)";
            var sqlQuery = @"INSERT INTO CustomersTABLE (Id, Name, Email) VALUES (UPPER(@Id), UPPER(@Name), UPPER(@Email));";

            using (SqlConnection connection = new SqlConnection(sqlConnection))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(countQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", customer.Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    int count = (int)await command.ExecuteScalarAsync();
                    if(count > 0) return new APIResponse() { Success = false, Message = "Customer already registered in the system.", Data = null };
                }

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", customer.Id);
                    command.Parameters.AddWithValue("@Name", customer.Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    await command.ExecuteNonQueryAsync();
                }
            }

            customer.Id = Utils.PrettyTextDisplay(customer.Id);
            customer.Name = Utils.PrettyTextDisplay(customer.Name);
            customer.Email = Utils.PrettyTextDisplay(customer.Email);
            return new APIResponse() { Success = true, Message = "Customer successfuly registered.", Data = customer };
        }
        public async Task<APIResponse> UpdateCustumer(Customer customer)
        {
            APIResponse result = new APIResponse() { Success = true, Message = "Customer data updated.", Data = customer };
            try
            {
                var sqlConnection = _configuration.GetConnectionString("connString");
                var countQuery = @"SELECT COUNT(*) FROM CustomersTABLE WHERE (Name = UPPER(@Name) AND Email = UPPER(@Email)) AND Id <> UPPER(@Id) ";
                var sqlQuery = @"UPDATE CustomersTABLE SET Name = UPPER(@Name), Email = UPPER(@Email) WHERE Id = UPPER(@Id);";

                using (SqlConnection connection = new SqlConnection(sqlConnection))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(countQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", customer.Id);
                        command.Parameters.AddWithValue("@Name", customer.Name);
                        command.Parameters.AddWithValue("@Email", customer.Email);
                        int count = (int)await command.ExecuteScalarAsync();

                        if (count > 0) return new APIResponse() { Success = false, Message = "The provided name or email already exists in the system.", Data = null };
                    }

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", customer.Id);
                        command.Parameters.AddWithValue("@Name", customer.Name);
                        command.Parameters.AddWithValue("@Email", customer.Email);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                customer.Id = Utils.PrettyTextDisplay(customer.Id);
                customer.Name = Utils.PrettyTextDisplay(customer.Name);
                customer.Email = Utils.PrettyTextDisplay(customer.Email);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<APIResponse> DeleteCustomer(string id)
        {
            APIResponse result = new APIResponse() { Success = true, Message = "Customer data removed.", Data = null };
            var sqlConnection = _configuration.GetConnectionString("connString");
            var sqlQuery = @"DELETE FROM CustomersTABLE WHERE Id = UPPER(@Id);";
            using (SqlConnection connection = new SqlConnection(sqlConnection))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int count = await command.ExecuteNonQueryAsync();
                    if (count <= 0)
                    { 
                        result = new APIResponse() { Success = false, Message = "Customer information not found.", Data=null };
                    }
                }
            }
            return result;
        }
    }
}
