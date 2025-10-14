using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class PriceBase
    {
        [JsonProperty("nano")]
        public int Nano { get; set; }

        [JsonProperty("units")]
        public decimal Units { get; set; }

        public decimal Total { get { return Units + Nano * 0.000000001m; } }
    }
}