using Domain;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace UnitTests.API
{
    public class DatabaseTest
    {
        private HttpClient _client;

        [Fact]
        public async Task PostCalculationsTest()
        {
            _client = new HttpClient();
            CalculationSettings calculationSettings = new()
            {
                Name = "testName",
                Description = "testDescription",
                PathToRegim = "testPath",
                PercentLoad = 10,
                PercentForWorsening = 15,
                CountOfImplementations = 5,
                //NodesForWorsening = new List<int>() { 2658, 2653, 1654, 60408134 },
                SechNumber = 2
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(calculationSettings), Encoding.UTF8, "application/json");
            HttpRequestMessage request = new()
            {
                RequestUri = new Uri("https://localhost:7231/api/CalculationPowerFlows/PostCalculations"),
                Method = HttpMethod.Post,
                Content = httpContent
            };
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            HttpResponseMessage response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
