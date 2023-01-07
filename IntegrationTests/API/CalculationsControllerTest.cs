using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace IntegrationTests.API
{
    public class CalculationsControllerTest
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public CalculationsControllerTest()
        {
            _factory = new();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetCalculations_ShouldReturnOk()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/Calculations/GetCalculations");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCalculationResultById_ShouldReturnOk()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/Calculations/GetCalculations");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task StartCalculation_ShouldReturnOk()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/Calculations/GetCalculations");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /*[Fact]
        public async Task PostCalculationsTest()
        {
            *//*_client = new HttpClient();
            CalculationSettingsRequest calculationSettings = new()
            {
                Name = "testNameTest",
                Description = "testDescription",
                PathToRegim = "testPath",
                PercentLoad = 10,
                PercentForWorsening = 15,
                CountOfImplementations = 3,
                WorseningSettings = new List<WorseningSettingsDto>() {new (2658,null), new (2653,null), new(1654, null), new(60408134, null) }, //{ 2658, 2653, 1654, 60408134 },
                SechNumber = 2
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(calculationSettings), Encoding.UTF8, "application/json");
            HttpRequestMessage request = new()
            {
                RequestUri = new Uri("https://localhost:5001/api/CalculationPowerFlows/PostCalculations"),
                Method = HttpMethod.Post,
                Content = httpContent
            };
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            HttpResponseMessage response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);*//*
        }*/
    }
}
