using Buratino.Xtensions;
using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class BondCoupons
    {
        [JsonProperty("events")]
        public List<Event> Events { get; set; }
    }

    public class Event
    {
        [JsonProperty("figi")]
        public string Figi { get; set; }

        [JsonProperty("couponDate")]
        public DateTime CouponDate { get; set; }

        [JsonProperty("couponNumber")]
        public string CouponNumber { get; set; }

        [JsonProperty("fixDate")]
        public DateTime FixDate { get; set; }

        [JsonProperty("payOneBond")]
        public PayOneBond PayOneBond { get; set; }

        [JsonProperty("couponType")]
        public string CouponType { get; set; }

        [JsonProperty("couponStartDate")]
        public DateTime CouponStartDate { get; set; }

        [JsonProperty("couponEndDate")]
        public DateTime CouponEndDate { get; set; }

        [JsonProperty("couponPeriod")]
        public int CouponPeriod { get; set; }

        public override string ToString()
        {
            return $"{FixDate:d} - {PayOneBond.Total.Round(1)}";
        }
    }

    public class PayOneBond : CurrencyPrice
    {

    }
}
