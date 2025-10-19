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

        /// <summary>
        /// Итоговая стоимость покупки с учетом НКД и коммисией брокера
        /// </summary>
        public decimal TotalPrice => FeeCoast + Instrument.AciValue.Total;

        public bool IsRUB { get; set; }
        public bool IsFixed { get; set; }
        public bool IsEquals { get; set; }
        public bool IsAnnounsed { get; set; }
        public bool IsAmortization { get; set; }

        public void CalcMarkers()
        {
            IsRUB = GetIsRUB();
            IsFixed = GetIsFixed();
            IsEquals = GetIsEquals();
            IsAnnounsed = GetIsAnnounsed();
            IsAmortization = GetIsAmortization();
        }

        public bool GetIsRUB()
        {
            if (Instrument.Nominal.Currency != "rub")
                return false;
            if (Coupons.Events.Any(x => x.PayOneBond.Currency != "rub"))
                return false;
            return true;
        }

        public bool GetIsFixed()
        {
            var coupons = Coupons.Events.OrderBy(x => x.CouponNumber).Where(x => x.CouponEndDate.Between_LTE_GTE(DateTime.Now, EndDate));
            if (!coupons.Any())
                return false;

            if (coupons.Any(x => x.CouponType != CouponTypes.COUPON_TYPE_FIX))
                return false;

            return true;
        }

        public bool GetIsEquals()
        {
            var coupons = Coupons.Events.OrderBy(x => x.CouponNumber).Where(x => x.CouponEndDate.Between_LTE_GTE(DateTime.Now, EndDate));
            if (!coupons.Any())
                return false;

            if (coupons.Any(x => x.PayOneBond.Total == 0))
                return false;

            var min = coupons.Min(x => x.PayOneBond.Total);
            var max = coupons.Max(x => x.PayOneBond.Total);
            var diff = max - min;

            //Чтобы купоны разных месяцев считались одинаковыми
            return diff / max < 0.10m;
        }

        public bool GetIsAnnounsed()
        {
            var coupons = Coupons.Events.OrderBy(x => x.CouponNumber).Where(x => x.CouponEndDate.Between_LTE_GTE(DateTime.Now, EndDate));
            if (!coupons.Any())
                return false;

            if (coupons.Any(x => x.PayOneBond.Total == 0))
                return false;
            return true;
        }

        public bool GetIsAmortization()
        {
            return Instrument.AmortizationFlag;
        }

        /// <summary>
        /// Можно ли посчитать доходность
        /// </summary>
        /// <returns></returns>
        public bool CanCalcProfit()
        {
            //TFTTF
            //TTTTF
            //TTFTF
            //TTTTT
            if (IsRUB && IsAnnounsed)
            {
                if (!IsFixed && IsEquals && !IsAmortization)
                {
                    return true;
                }
                if (IsFixed && IsEquals && !IsAmortization)
                {
                    return true;
                }
                if (IsFixed && !IsEquals && !IsAmortization)
                {
                    return true;
                }
                if (IsFixed && IsEquals && IsAmortization)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public decimal GetCouponSum()
        {
            var coupons = Coupons.Events.OrderBy(x => x.CouponNumber).Where(x => x.CouponEndDate.Between_LTE_GTE(DateTime.Now, EndDate));
            if (!coupons.Any())
                return 0;

            return coupons.Sum(x => x.PayOneBond.Total) - Instrument.AciValue.Total;
        }

        public decimal GetYearlyIncome()
        {
            if (!CanCalcProfit())
                return 0;

            var daysToEnd = (decimal)EndDate.Subtract(DateTime.Now).TotalDays;
            var totalOutcome = TotalPrice;

            var totalIncome = GetCouponSum() + Instrument.Nominal.Total;
            var diffIncome = totalIncome - totalOutcome;
            var yearlyIncome = diffIncome / totalOutcome / daysToEnd * 365.25m * 100;
            return yearlyIncome;
        }
    }
}
