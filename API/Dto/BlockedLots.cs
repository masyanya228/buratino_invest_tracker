using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class BlockedLots
    {
        [JsonProperty("units")]
        public decimal Units { get; set; }

        [JsonProperty("nano")]
        public int Nano { get; set; }
    }
}
