namespace Buratino.Services.Dto
{
    public class InvestStatsDto
    {
        public decimal Percent { get; internal set; }
        public decimal Balance { get; internal set; }
        public decimal IncomePerMonth { get; internal set; }
        public decimal IncomePerYear { get; internal set; }
    }
}