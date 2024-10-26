using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Money
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("units")]
        public string Units { get; set; }

        [JsonProperty("nano")]
        public int Nano { get; set; }
    }


}
