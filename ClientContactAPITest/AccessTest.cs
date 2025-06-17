using ClientContactAPI.Classes;
using ClientContactAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace ClientContactAPITest
{
    public class AccessTest : IClassFixture<WebApplicationFactory<ClientContactAPI.Program>>
    {
        private readonly HttpClient _client;
        public AccessTest(WebApplicationFactory<ClientContactAPI.Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task Login_Test()
        {
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

            Assert.True(apiLoginResponse.Success);
            Assert.NotNull(apiLoginResponse.Data);
        }
    }
}
