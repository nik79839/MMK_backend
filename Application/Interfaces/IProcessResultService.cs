using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Application.Interfaces
{
    public interface IProcessResultService
    {
        IEnumerable<CurrentResultProcessed> Processing(List<CurrentResult> currentResults);
        PowerFlowResultProcessed Processing(List<PowerFlowResult> powerFlowResults);
        IEnumerable<VoltageResultProcessed> Processing(List<VoltageResult> voltageResults);
    }
}