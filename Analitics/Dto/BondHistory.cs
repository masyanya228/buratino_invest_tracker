using Buratino.API.Dto;

namespace Buratino.Analitics.Dto
{
    /// <summary>
    /// История владения облигацией
    /// </summary>
    public class BondHistory
    {
        public Position Position { get; set; }

        /// <summary>
        /// Полная стоимость покупки
        /// С учетом комиссии и НКД
        /// </summary>
        public decimal TotalBuyPrice { get; set; }

        public decimal AvgBuyPrice => TotalBuyPrice / Position.QuantityLots.Total;

        public decimal TotalSellPrice { get; set; }

        /// <summary>
        /// Сумма всех выплат
        /// </summary>
        public decimal TotalIncome => Coupons.Sum(x => x.Payment.GetInRub());

        public decimal Progress => TotalIncome / -TotalBuyPrice * 100;

        public decimal Diff => TotalSellPrice + AvgBuyPrice;

        public Instrument? Bond { get; set; }

        public Item[] Operations { get; set; }

        public Item[] Coupons
        {
            get
            {
                return Operations
                    .Where(x => x.Type == Enums.OperationType.OPERATION_TYPE_COUPON
                                || x.Type == Enums.OperationType.OPERATION_TYPE_TAX_CORRECTION_COUPON
                                || x.Type == Enums.OperationType.OPERATION_TYPE_BOND_TAX
                                || x.Type == Enums.OperationType.OPERATION_TYPE_BOND_TAX_PROGRESSIVE
                                || x.Type == Enums.OperationType.OPERATION_TYPE_BOND_REPAYMENT
                                || x.Type == Enums.OperationType.OPERATION_TYPE_BOND_REPAYMENT_FULL)
                    .ToArray();
            }
        }

        public override string ToString()
        {
            return Bond?.Name ?? Position.Ticker;
        }
    }
}
