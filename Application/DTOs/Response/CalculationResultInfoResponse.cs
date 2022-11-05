using Application.DTOs.InitialResult;
using Application.DTOs.ProcessedResult;
using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Application.DTOs.Response
{
    /// <summary>
    /// Результат расчета, отправляемый клиенту
    /// </summary>
    public class CalculationResultInfoResponse
    {
        public CalculationResultInitialDto InitialResult { get; set; }
        public CalculationResultProcessedDto ProcessedResult { get; set; }

        public CalculationResultInfoResponse(CalculationResultInitialDto initialResult, CalculationResultProcessedDto processedResult)
        {
            InitialResult = initialResult;
            ProcessedResult = processedResult;
        }
    }
}
