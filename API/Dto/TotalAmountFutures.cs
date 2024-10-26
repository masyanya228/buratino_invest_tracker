using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class TotalAmountFutures
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("units")]
        public decimal Units { get; set; }

        [JsonProperty("nano")]
        public int Nano { get; set; }
    }
}
