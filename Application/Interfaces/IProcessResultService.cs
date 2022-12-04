using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Application.Interfaces
{
    public interface IProcessResultService
    {
        List<CurrentResultProcessed> Processing(List<CurrentResult> currentResults);
        StatisticBase Processing(List<PowerFlowResult> powerFlowResults);
        List<VoltageResultProcessed> Processing(List<VoltageResult> voltageResults);
    }
}