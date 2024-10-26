namespace Buratino.Models.Services.Dto
{
    public class PlanChargeDto
    {
        public int AmountOfCharges { get; set; }

        public decimal TotalFact { get; set; }

        public double TodayPlan { get; set; }

        public decimal ChargeProgress { get; set; }

        public double YearProgress { get; set; }
    }
}