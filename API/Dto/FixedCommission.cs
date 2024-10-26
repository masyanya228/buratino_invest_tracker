using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class FixedCommission
    {
        [JsonProperty("units")]
        public string Units { get; set; }

        [JsonProperty("nano")]
        public int Nano { get; set; }
    }
}
