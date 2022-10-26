using Domain;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Tests.API
{
    public class CalculationPowerFlowControllerTest
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new();
        }

        [Test]
        public async Task GetCalculationsTest()
        {
            _client = new HttpClient();
            var response = await _client.GetAsync("https://localhost:7231/CalculationPowerFlows/GetCalculations");
            Assert.AreEqual(HttpStatusCode.OK,response.StatusCode);
        }

        [Test]
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
                NodesForWorsening = new List<int>() { 2658,2653,1654, 60408134},
                SechNumber = 2
            };

            var httpContent = new StringContent(JsonConvert.SerializeObject(calculationSettings), Encoding.UTF8, "application/json");
            HttpRequestMessage request = new ();
            request.RequestUri = new Uri("https://localhost:7231/CalculationPowerFlows/PostCalculations");
            request.Method = HttpMethod.Post;
            request.Content = httpContent;
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            HttpResponseMessage response = await _client.SendAsync(request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
