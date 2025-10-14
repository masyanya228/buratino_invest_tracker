using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Bonds
    {
        [JsonProperty("instruments")]
        public List<Instrument> Instruments { get; set; }
    }
}
