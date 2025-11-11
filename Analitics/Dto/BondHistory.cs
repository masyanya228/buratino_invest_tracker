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

        public decimal TotalSellPrice { get; set; }

        public decimal Diff => TotalSellPrice + TotalBuyPrice;

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
