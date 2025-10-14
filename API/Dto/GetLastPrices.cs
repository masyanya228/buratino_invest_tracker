using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class GetLastPrices
    {
        [JsonProperty("lastPrices")]
        public List<LastPrice> LastPrices { get; set; }
    }

    public class LastPrice
    {
        [JsonProperty("figi")]
        public string Figi { get; set; }

        [JsonProperty("price")]
        public PriceBase Price { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("instrumentUid")]
        public string InstrumentUid { get; set; }

        [JsonProperty("lastPriceType")]
        public string LastPriceType { get; set; }
    }
}
