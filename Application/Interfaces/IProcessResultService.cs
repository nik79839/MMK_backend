using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Application.Interfaces
{
    public interface IProcessResultService
    {
        IEnumerable<StatisticBase> Processing(List<CurrentResult> currentResults);
        StatisticBase Processing(List<CalculationResultBase> powerFlowResults);
        IEnumerable<StatisticBase> Processing(List<VoltageResult> voltageResults);
    }
}