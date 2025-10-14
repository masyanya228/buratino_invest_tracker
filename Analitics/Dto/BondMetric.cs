using Buratino.API.Dto;
using Buratino.Xtensions;

namespace Buratino.Analitics.Dto
{
    public class BondMetric
    {
        public Instrument Instrument { get; set; }
        public BondCoupons Coupons { get; set; }
        public PriceBase LastPrice { get; set; }

        /// <summary>
        /// Погашение либо оферта
        /// </summary>
        public DateTime EndDate => Instrument.CallDate ?? Instrument.MaturityDate;

        /// <summary>
        /// Стоимость покупки с учетом коммисии брокера
        /// </summary>
        public decimal FeeCoast => (LastPrice?.Total ?? 100.0m) / 100.0m * Instrument.Nominal.Total * 1.003m;

        public bool IsFixedCouponsToEnd()
        {
            if (Instrument.Nominal.Currency != "rub")
                return false;

            var coupons = Coupons.Events.OrderBy(x => x.CouponNumber).Where(x => x.CouponEndDate.Between_LTE_GTE(DateTime.Now, EndDate));
            if (!coupons.Any())
                return false;

            var min = coupons.Min(x => x.PayOneBond.Total);
            var max = coupons.Max(x => x.PayOneBond.Total);
            var diff = max - min;
            return diff / max < 0.10m;
        }

        public bool IsAnnouncedCouponsToEnd()
        {
            var coupons = Coupons.Events.OrderBy(x => x.CouponNumber).Where(x => x.CouponEndDate.Between_LTE_GTE(DateTime.Now, EndDate));
            if (!coupons.Any())
                return false;

            return coupons.All(x => x.PayOneBond.Total > 0);
        }

        public decimal GetCouponSum()
        {
            var coupons = Coupons.Events.OrderBy(x => x.CouponNumber).Where(x => x.CouponEndDate.Between_LTE_GTE(DateTime.Now, EndDate));
            if (!coupons.Any())
                return 0;

            return coupons.Sum(x => x.PayOneBond.Total) - Instrument.AciValue.Total;
        }

        /// <summary>
        /// Итоговая стоимость покупки с учетом НКД и коммисией брокера
        /// </summary>
        public decimal TotalPrice => FeeCoast + Instrument.AciValue.Total;

        public decimal GetYearlyIncome()
        {
            if (!IsFixedCouponsToEnd() || !IsAnnouncedCouponsToEnd())
                return 0;

            var daysToEnd = (decimal)EndDate.Subtract(DateTime.Now).TotalDays;

            var totalIncome = GetCouponSum() + Instrument.Nominal.Total;
            var totalOutcome = TotalPrice;
            var diffIncome = totalIncome - totalOutcome;
            var yearlyIncome = diffIncome / totalOutcome / daysToEnd * 365.25m * 100;
            return yearlyIncome;
        }
    }
}
