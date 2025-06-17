using ClientContactAPI;
using ClientContactAPI.Classes;
using ClientContactAPI.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Utils = ClientContactAPI.Utils;

namespace ClientContactAPITest
{
    public class CustomerControllerTest : IClassFixture<WebApplicationFactory<ClientContactAPI.Program>>
    {
        private readonly HttpClient _client;

        public CustomerControllerTest(WebApplicationFactory<ClientContactAPI.Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetCustomerList_Test()
        {
            //GET ACCESS TOKEN
            string accessToken = "";
            var login = new
            {
                Username = "admin".ToUpper(),
                HashPassword = Utils.HashGenerator("Password123").ToUpper(),
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Access/Login", login);
            loginResponse.EnsureSuccessStatusCode();

            var responseStringLogin = await loginResponse.Content.ReadAsStringAsync();
            var apiLoginResponse = JsonConvert.DeserializeObject<APIResponse>(responseStringLogin);
            if (apiLoginResponse.Success)
            {
                accessToken = apiLoginResponse.Data.ToString();
            }
            //GET LIST TEST
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _client.GetAsync("/api/CustomerManagement/");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<APIResponse>(responseString);

            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Success);
            Assert.Equal("", apiResponse.Message);
        }
        [Fact]
        public async Task RegisterCustomer_Test()
        {
            //GET ACCESS TOKEN
            string accessToken = "";
            var login = new
            {
                Username = "admin".ToUpper(),
                HashPassword = Utils.HashGenerator("Password123").ToUpper(),
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Access/Login", login);
            loginResponse.EnsureSuccessStatusCode();

            var responseStringLogin = await loginResponse.Content.ReadAsStringAsync();
            var apiLoginResponse = JsonConvert.DeserializeObject<APIResponse>(responseStringLogin);
            if (apiLoginResponse.Success)
            {
                accessToken = apiLoginResponse.Data.ToString();
            }
            //TEST
            var uniqueValue = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newCustomer = new
            {
                Name = $"Testuser_{uniqueValue}",
                Email = $"testuser_{uniqueValue}@gmail.com"
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",accessToken);
            var registerResponse = await _client.PostAsJsonAsync("/api/CustomerManagement/", newCustomer);
            registerResponse.EnsureSuccessStatusCode();

            var registerContent = await registerResponse.Content.ReadAsStringAsync();
            var registeredResult = JsonConvert.DeserializeObject<APIResponse>(registerContent);

 
            var customerObj = ((JObject)registeredResult.Data).ToObject<Customer>();
 
            Assert.NotNull(customerObj);
            Assert.NotNull(customerObj.Name);
            Assert.NotNull(customerObj.Email);
            Assert.True(registeredResult.Success);
            Assert.Equal("Customer successfuly registered.", registeredResult.Message);
        }
        [Fact]
        public async Task GetCustomer_Test()
        {
            //GET ACCESS TOKEN
            string accessToken = "";
            var login = new
            {
                Username = "admin".ToUpper(),
                HashPassword = Utils.HashGenerator("Password123").ToUpper(),
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Access/Login", login);
            loginResponse.EnsureSuccessStatusCode();

            var responseStringLogin = await loginResponse.Content.ReadAsStringAsync();
            var apiLoginResponse = JsonConvert.DeserializeObject<APIResponse>(responseStringLogin);
            if (apiLoginResponse.Success)
            {
                accessToken = apiLoginResponse.Data.ToString();
            }
            //TEST
            var uniqueValue = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newCustomer = new
            {
                Name = $"Testuser_{uniqueValue}",
                Email = $"Testuser_{uniqueValue}@gmail.com"
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var registerResponse = await _client.PostAsJsonAsync("/api/CustomerManagement/", newCustomer);
            registerResponse.EnsureSuccessStatusCode();

            var registerContent = await registerResponse.Content.ReadAsStringAsync();
            var registeredResult = JsonConvert.DeserializeObject<APIResponse>(registerContent);
            var newCustomerObj = ((JObject)registeredResult.Data).ToObject<Customer>();

            var getResponse = await _client.GetAsync($"/api/CustomerManagement/{newCustomerObj.Id}");
            getResponse.EnsureSuccessStatusCode();

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var getResult = JsonConvert.DeserializeObject<APIResponse>(getContent);
            var fetchedCustomer = ((JObject)getResult.Data).ToObject<Customer>(); 

            Assert.NotNull(fetchedCustomer);
            Assert.Equal(newCustomer.Name, fetchedCustomer.Name);
            Assert.Equal(newCustomer.Email, fetchedCustomer.Email);
            Assert.True(getResult.Success);
            Assert.Equal("", getResult.Message);
        }
        [Fact]
        public async Task UpdateCustomer_Test()
        {
            //GET ACCESS TOKEN
            string accessToken = "";
            var login = new
            {
                Username = "admin".ToUpper(),
                HashPassword = Utils.HashGenerator("Password123").ToUpper(),
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Access/Login", login);
            loginResponse.EnsureSuccessStatusCode();

            var responseStringLogin = await loginResponse.Content.ReadAsStringAsync();
            var apiLoginResponse = JsonConvert.DeserializeObject<APIResponse>(responseStringLogin);
            if (apiLoginResponse.Success)
            {
                accessToken = apiLoginResponse.Data.ToString();
            }
            //TEST
            var uniqueValue = Guid.NewGuid().ToString("N").Substring(0, 8).ToLower();
            var newCustomer = new
            {
                Name = $"Testuser_{uniqueValue}",
                Email = $"Testuser_{uniqueValue}@gmail.com"
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var registerResponse = await _client.PostAsJsonAsync("/api/CustomerManagement/", newCustomer);
            registerResponse.EnsureSuccessStatusCode();
            var registerContent = await registerResponse.Content.ReadAsStringAsync();
            var registeredResult = JsonConvert.DeserializeObject<APIResponse>(registerContent);
            var createdCustomer = ((JObject)registeredResult.Data).ToObject<Customer>();

            var updatedCustomer = new
            {
                Name = createdCustomer.Name + "_updated",
                Email = "Updated_" + createdCustomer.Email.ToLower()
            };

            var updateResponse = await _client.PutAsJsonAsync($"/api/CustomerManagement/{createdCustomer.Id}", updatedCustomer);
            updateResponse.EnsureSuccessStatusCode();
            var updateContent = await updateResponse.Content.ReadAsStringAsync();
            var updateResult = JsonConvert.DeserializeObject<APIResponse>(updateContent);
            var updatedCustomerObj = ((JObject)updateResult.Data).ToObject<Customer>();

            Assert.NotNull(updatedCustomerObj);
            Assert.Equal(updatedCustomer.Name, updatedCustomerObj.Name);
            Assert.Equal(updatedCustomer.Email, updatedCustomerObj.Email);
            Assert.True(updateResult.Success);
            Assert.Equal("Customer data updated.", updateResult.Message);
        }
        [Fact]
        public async Task DeleteCustomer_Test()
        {
            //GET ACCESS TOKEN
            string accessToken = "";
            var login = new
            {
                Username = "admin".ToUpper(),
                HashPassword = Utils.HashGenerator("Password123").ToUpper(),
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Access/Login", login);
            loginResponse.EnsureSuccessStatusCode();

            var responseStringLogin = await loginResponse.Content.ReadAsStringAsync();
            var apiLoginResponse = JsonConvert.DeserializeObject<APIResponse>(responseStringLogin);
            if (apiLoginResponse.Success)
            {
                accessToken = apiLoginResponse.Data.ToString();
            }
            //TEST
            var uniqueValue = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newCustomer = new
            {
                Name = $"Testuser_{uniqueValue}",
                Email = $"testuser_{uniqueValue}@gmail.com"
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var registerResponse = await _client.PostAsJsonAsync("/api/CustomerManagement/", newCustomer);
            registerResponse.EnsureSuccessStatusCode();
            var registerContent = await registerResponse.Content.ReadAsStringAsync();
            var registerResult = JsonConvert.DeserializeObject<APIResponse>(registerContent);
            var createdCustomer = ((JObject)registerResult.Data).ToObject<Customer>();

            var deleteResponse = await _client.DeleteAsync($"/api/CustomerManagement/{createdCustomer.Id}");
            deleteResponse.EnsureSuccessStatusCode();
            var deleteContent = await deleteResponse.Content.ReadAsStringAsync();
            var deleteResult = JsonConvert.DeserializeObject<APIResponse>(deleteContent);

            Assert.True(deleteResult.Success);
            Assert.Equal("Customer data removed.", deleteResult.Message);

            var getResponse = await _client.GetAsync($"/api/CustomerManagement/{createdCustomer.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var getResult = JsonConvert.DeserializeObject<APIResponse>(getContent);

            Assert.False(getResult.Success);
            Assert.Equal("Customer not found.", getResult.Message);
            Assert.Null(getResult.Data);
        }
    }
}