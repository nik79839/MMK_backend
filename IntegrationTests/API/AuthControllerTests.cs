using Application.DTOs.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace IntegrationTests.API
{
    public class AuthControllerTests
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public AuthControllerTests()
        {
            _factory = new();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/Auth/GetUsers");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnOk()
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new LoginRequest() { Login = "admin", Password = "admin" }),
                Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("https://localhost:5001/api/Auth/auth", httpContent);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
