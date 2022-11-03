using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Domain
{
    /// <summary>
    /// Результат расчета, отправляемый клиенту
    /// </summary>
    public class CalculationResultInfo
    {
        public CalculationResultInitial InitialResult {get; set;}
        public CalculationResultProcessed ProcessedResult { get; set; }
        public List<int> WorseningSettings { get; set; }

        public CalculationResultInfo(CalculationResultInitial initialResult, CalculationResultProcessed processedResult, List<int> worseningSettings)
        {
            InitialResult = initialResult;
            ProcessedResult = processedResult;
            WorseningSettings = worseningSettings;
        }
    }
}
