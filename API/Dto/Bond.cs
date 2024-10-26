using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Bond
    {
        [JsonProperty("instrument")]
        public Instrument Instrument { get; set; }
    }
}
