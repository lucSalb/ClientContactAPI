using ClientContactAPI.Classes;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using ClientContactAPI;

namespace ClientContactAPITest
{
    public class CommunicationControllerTest : IClassFixture<WebApplicationFactory<ClientContactAPI.Program>>
    {
        private readonly HttpClient _client;
        public CommunicationControllerTest(WebApplicationFactory<ClientContactAPI.Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task SendCommunication_Test()
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
            var registeredResult = JsonConvert.DeserializeObject<APIResponse>(registerContent);

            var customer = ((JObject)registeredResult.Data).ToObject<Customer>();


            var unique = Guid.NewGuid().ToString("N")[..8];
            var newTemplate = new
            {
                Name = $"Testtemplate_{unique}",
                Subject = "Test Subject",
                Body = "This is a test template body for [NAME] to [EMAIL]."
            };

            var response = await _client.PostAsJsonAsync("/api/TemplateManagement/", newTemplate);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<APIResponse>(content);

            var template = ((JObject)result.Data).ToObject<Template>();
             
            var newCommunication = new
            {
                CustomerId = customer.Id,
                TemplateId = template.Id
            };

            var communicationResponse = await _client.PostAsJsonAsync("/api/SendCommunication/", newCommunication);
            communicationResponse.EnsureSuccessStatusCode();

            var sendContent = await communicationResponse.Content.ReadAsStringAsync();
            var sendResult = JsonConvert.DeserializeObject<APIResponse>(sendContent);
             

            Assert.NotNull(sendResult);
            Assert.Null(sendResult.Data);
            Assert.Equal(sendResult.Message, $"E-mail sent to {customer.Name}");
            Assert.True(sendResult.Success); 
        }
        [Fact]
        public async Task SendCommunicationAll_Test()
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

            var unique = Guid.NewGuid().ToString("N")[..8];
            var newTemplate = new
            {
                Name = $"Testtemplate_{unique}",
                Subject = "Test Subject",
                Body = "This is a test template body for [NAME] to [EMAIL]."
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.PostAsJsonAsync("/api/TemplateManagement/", newTemplate);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<APIResponse>(content);

            var template = ((JObject)result.Data).ToObject<Template>();

            var newAllCommunication = new
            {
                TemplateId = template.Id
            };

            var communicationResponse = await _client.PostAsJsonAsync("/api/SendCommunication/broadcast", newAllCommunication);
            communicationResponse.EnsureSuccessStatusCode();

            var sendContent = await communicationResponse.Content.ReadAsStringAsync();
            var sendResult = JsonConvert.DeserializeObject<APIResponse>(sendContent);


            Assert.NotNull(sendResult);
            Assert.True(sendResult.Success);
        }
    }
}
