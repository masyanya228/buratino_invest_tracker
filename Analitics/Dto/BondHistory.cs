using Buratino.API.Dto;

namespace Buratino.Analitics.Dto
{
    public class BondHistory
    {
        public Position Position { get; set; }

        public decimal TotalBuyPrice { get; set; }

        public decimal TotalSellPrice { get; set; }

        public decimal Diff => TotalSellPrice + TotalBuyPrice;
    }
}
