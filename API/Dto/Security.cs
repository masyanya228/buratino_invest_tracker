using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Security
    {
        [JsonProperty("figi")]
        public string Figi { get; set; }

        [JsonProperty("blocked")]
        public decimal Blocked { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("positionUid")]
        public string PositionUid { get; set; }

        [JsonProperty("instrumentUid")]
        public string InstrumentUid { get; set; }

        [JsonProperty("exchangeBlocked")]
        public bool ExchangeBlocked { get; set; }

        [JsonProperty("instrumentType")]
        public string InstrumentType { get; set; }
    }
}
