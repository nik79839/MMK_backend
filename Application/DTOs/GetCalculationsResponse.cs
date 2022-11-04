namespace Application.DTOs
{
    public class GetCalculationsResponse
    {
        public int CalculationAmount { get; set; }
        public List<CalculationDto> Calculations { get; set; } = new();
    }
}
