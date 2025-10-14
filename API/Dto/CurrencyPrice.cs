using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class CurrencyPrice : PriceBase
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        public decimal GetInRub()
        {
            if (Currency == "rub")
                return Units;
            if (Currency == "usd")
                return Units * 80.85m;
            throw new ArgumentOutOfRangeException();
        }
    }
}