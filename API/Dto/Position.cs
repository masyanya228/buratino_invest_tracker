using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Position
    {
        [JsonProperty("figi")]
        public string Figi { get; set; }

        [JsonProperty("instrumentType")]
        public string InstrumentType { get; set; }

        [JsonProperty("quantity")]
        public Quantity Quantity { get; set; }

        [JsonProperty("averagePositionPrice")]
        public AveragePositionPrice AveragePositionPrice { get; set; }

        [JsonProperty("expectedYield")]
        public ExpectedYield ExpectedYield { get; set; }

        [JsonProperty("averagePositionPricePt")]
        public AveragePositionPricePt AveragePositionPricePt { get; set; }

        [JsonProperty("currentPrice")]
        public CurrentPrice CurrentPrice { get; set; }

        [JsonProperty("averagePositionPriceFifo")]
        public AveragePositionPriceFifo AveragePositionPriceFifo { get; set; }

        [JsonProperty("quantityLots")]
        public QuantityLots QuantityLots { get; set; }

        [JsonProperty("blocked")]
        public bool Blocked { get; set; }

        [JsonProperty("blockedLots")]
        public BlockedLots BlockedLots { get; set; }

        [JsonProperty("positionUid")]
        public string PositionUid { get; set; }

        [JsonProperty("instrumentUid")]
        public string InstrumentUid { get; set; }

        [JsonProperty("varMargin")]
        public VarMargin VarMargin { get; set; }

        [JsonProperty("expectedYieldFifo")]
        public ExpectedYieldFifo ExpectedYieldFifo { get; set; }

        [JsonProperty("currentNkd")]
        public CurrentNkd CurrentNkd { get; set; }

        [JsonProperty("ticker")]
        public string Ticker { get; set; }
    }
}
