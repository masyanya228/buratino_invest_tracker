using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public partial class Instrument
    {
        [JsonProperty("fixedCommission")]
        public FixedCommission FixedCommission { get; set; }

        [JsonProperty("focusType")]
        public string FocusType { get; set; }

        [JsonProperty("releasedDate")]
        public DateTime ReleasedDate { get; set; }

        [JsonProperty("rebalancingFreq")]
        public string RebalancingFreq { get; set; }

        [JsonProperty("instrumentExchange")]
        public string InstrumentExchange { get; set; }
    }
}
