using Application.Interfaces;
using Domain;
using Domain.InitialResult;
using Domain.ProcessedResult;
using Infrastructure.Services;

namespace UnitTests
{
    public class ProcessResultServiceTests
    {
        [Fact]
        public async Task Processing_ShouldReturnProcessedResults()
        {
            var service = new ProcessResultService();
            var result = service.Processing(new List<PowerFlowResult>() { new PowerFlowResult(Guid.NewGuid(), 1, 100) });

            Assert.IsType<PowerFlowResultProcessed>(result);
        }
    }
}
