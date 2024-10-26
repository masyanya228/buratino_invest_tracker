using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Quantity
    {
        [JsonProperty("units")]
        public decimal Units { get; set; }

        [JsonProperty("nano")]
        public int Nano { get; set; }
    }
}
