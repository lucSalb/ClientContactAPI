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
using ClientContactAPI;
using Utils = ClientContactAPI.Utils;
using Azure.Core;

namespace ClientContactAPITest
{
    public class TemplateControllerTest : IClassFixture<WebApplicationFactory<ClientContactAPI.Program>>
    {
        private readonly HttpClient _client;
        public TemplateControllerTest(WebApplicationFactory<ClientContactAPI.Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetTemplateList_Test()
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

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.GetAsync("/api/TemplateManagement/");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<APIResponse>(content);

            Assert.NotNull(result.Data);
            Assert.True(result.Success);
            Assert.Equal("", result.Message);
        }
        [Fact]
        public async Task RegisterTemplate_Test()
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
                Body = "This is a test template body."
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.PostAsJsonAsync("/api/TemplateManagement/", newTemplate);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<APIResponse>(content);

            var template = ((JObject)result.Data).ToObject<Template>();

            Assert.NotNull(template);
            Assert.Equal(newTemplate.Name, template.Name);
            Assert.Equal(newTemplate.Subject, template.Subject);
            Assert.Equal(newTemplate.Body, template.Body);
            Assert.True(result.Success);
        }
        [Fact]
        public async Task GetTemplate_Test()
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
                Subject = "Subject A",
                Body = "Body A"
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var regResponse = await _client.PostAsJsonAsync("/api/TemplateManagement/", newTemplate);
            regResponse.EnsureSuccessStatusCode();

            var regContent = await regResponse.Content.ReadAsStringAsync();
            var regResult = JsonConvert.DeserializeObject<APIResponse>(regContent);
            var regTemplate = ((JObject)regResult.Data).ToObject<Template>();

            var getResponse = await _client.GetAsync($"/api/TemplateManagement/{regTemplate.Id}");
            getResponse.EnsureSuccessStatusCode();

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var getResult = JsonConvert.DeserializeObject<APIResponse>(getContent);
            var fetchedTemplate = ((JObject)getResult.Data).ToObject<Template>();

            Assert.Equal(regTemplate.Id, fetchedTemplate.Id);
            Assert.Equal(newTemplate.Name, fetchedTemplate.Name);
            Assert.Equal(newTemplate.Subject, fetchedTemplate.Subject);
            Assert.Equal(newTemplate.Body, fetchedTemplate.Body);
        }
        [Fact]
        public async Task UpdateTemplate_Test()
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
                Name = $"TestTemplate_{unique}",
                Subject = "Original Subject",
                Body = "Original Body"
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var regResponse = await _client.PostAsJsonAsync("/api/TemplateManagement/", newTemplate);
            regResponse.EnsureSuccessStatusCode();

            var regContent = await regResponse.Content.ReadAsStringAsync();
            var regResult = JsonConvert.DeserializeObject<APIResponse>(regContent);
            var regTemplate = ((JObject)regResult.Data).ToObject<Template>();

            var updatedTemplate = new
            {
                Name = $"Updated_{unique}",
                Subject = "Updated Subject",
                Body = "Updated Body"
            };

            var updateResponse = await _client.PutAsJsonAsync($"/api/TemplateManagement/{regTemplate.Id}", updatedTemplate);
            updateResponse.EnsureSuccessStatusCode();

            var updateContent = await updateResponse.Content.ReadAsStringAsync();
            var updateResult = JsonConvert.DeserializeObject<APIResponse>(updateContent);
            var updatedTemplateObj = ((JObject)updateResult.Data).ToObject<Template>();

            Assert.Equal(updatedTemplate.Name, updatedTemplateObj.Name);
            Assert.Equal(updatedTemplate.Subject, updatedTemplateObj.Subject);
            Assert.Equal(updatedTemplate.Body, updatedTemplateObj.Body);
        }
        [Fact]
        public async Task RemoveTemplate_Test()
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
                Name = $"TestTemplate_{unique}",
                Subject = "To be deleted",
                Body = "To be deleted body"
            };

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var regResponse = await _client.PostAsJsonAsync("/api/TemplateManagement/", newTemplate);
            regResponse.EnsureSuccessStatusCode();

            var regContent = await regResponse.Content.ReadAsStringAsync();
            var regResult = JsonConvert.DeserializeObject<APIResponse>(regContent);
            var regTemplate = ((JObject)regResult.Data).ToObject<Template>();

            var deleteResponse = await _client.DeleteAsync($"/api/TemplateManagement/{regTemplate.Id}");
            deleteResponse.EnsureSuccessStatusCode();

            var deleteContent = await deleteResponse.Content.ReadAsStringAsync();
            var deleteResult = JsonConvert.DeserializeObject<APIResponse>(deleteContent);

            Assert.True(deleteResult.Success);
        }
    }
}
