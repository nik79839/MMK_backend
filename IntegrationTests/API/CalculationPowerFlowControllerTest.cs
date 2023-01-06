using Application.DTOs;
using Domain;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace UnitTests.API
{
    public class CalculationPowerFlowControllerTest
    {
        private HttpClient _client;

        [Fact]
        public async Task GetCalculationsTest()
        {
            _client = new HttpClient();
            var response = await _client.GetAsync("https://localhost:7231/api/CalculationPowerFlows/GetCalculations");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostCalculationsTest()
        {
            /*_client = new HttpClient();
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
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);*/
        }
    }
}
