namespace Application.DTOs
{
    public class GetCalculationResultByIdResponse
    {
        public CalculationResultInfoDto CalculationResultInfo { get; set; }
        public CalculationSettingsDto calculationSettings { get; set; }
    }
}
