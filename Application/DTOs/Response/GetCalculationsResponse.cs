namespace Application.DTOs.Response
{
    public class GetCalculationsResponse
    {
        public int CalculationAmount { get; set; }
        public List<CalculationDto> Calculations { get; set; } = new();
    }
}
