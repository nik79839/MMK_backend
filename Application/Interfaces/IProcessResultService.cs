using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Application.Interfaces
{
    public interface IProcessResultService
    {
        IEnumerable<StatisticBase> Processing(List<CurrentResult> currentResults);
        PowerFlowResultProcessed Processing(List<PowerFlowResult> powerFlowResults);
        IEnumerable<StatisticBase> Processing(List<VoltageResult> voltageResults);
    }
}