using System.Net;

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
    }
}
