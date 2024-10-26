using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Etf
    {
        [JsonProperty("instrument")]
        public Instrument Instrument { get; set; }
    }
}
