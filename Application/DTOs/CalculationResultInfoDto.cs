using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Application.DTOs
{
    /// <summary>
    /// Результат расчета, отправляемый клиенту
    /// </summary>
    public class CalculationResultInfoDto
    {
        public CalculationResultInitial InitialResult { get; set; }
        public CalculationResultProcessed ProcessedResult { get; set; }
        public List<int> WorseningSettings { get; set; }

        public CalculationResultInfoDto(CalculationResultInitial initialResult, CalculationResultProcessed processedResult, List<int> worseningSettings)
        {
            InitialResult = initialResult;
            ProcessedResult = processedResult;
            WorseningSettings = worseningSettings;
        }
    }
}
