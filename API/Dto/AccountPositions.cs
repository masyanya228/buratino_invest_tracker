using Newtonsoft.Json;

namespace Buratino.API.Dto
{

    public class AccountPositions
    {
        [JsonProperty("money")]
        public List<Money> Money { get; set; }

        [JsonProperty("blocked")]
        public List<object> Blocked { get; set; }

        [JsonProperty("securities")]
        public List<Security> Securities { get; set; }

        [JsonProperty("limitsLoadingInProgress")]
        public bool LimitsLoadingInProgress { get; set; }

        [JsonProperty("futures")]
        public List<object> Futures { get; set; }

        [JsonProperty("options")]
        public List<object> Options { get; set; }
    }
}
