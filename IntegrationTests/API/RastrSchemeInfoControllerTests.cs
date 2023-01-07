using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace IntegrationTests.API
{
    public class RastrSchemeInfoControllerTests
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public RastrSchemeInfoControllerTests()
        {
            _factory = new();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetRastrSchemeInfo_ShouldReturnOk()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/RastrSchemeInfo/GetRastrSchemeInfo");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
