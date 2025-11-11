using Buratino.Enums;
using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class AccruedInt : CurrencyPrice
    {

    }

    public class ChildOperation
    {
        [JsonProperty("instrumentUid")]
        public string InstrumentUid { get; set; }

        [JsonProperty("payment")]
        public Payment Payment { get; set; }
    }

    public class Commission : CurrencyPrice
    {

    }

    public class Item
    {
        [JsonProperty("type")]
        public OperationType Type{ get; set; }

        [JsonProperty("cursor")]
        public string Cursor { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("assetUid")]
        public string AssetUid { get; set; }

        [JsonProperty("brokerAccountId")]
        public string BrokerAccountId { get; set; }

        [JsonProperty("accruedInt")]
        public AccruedInt AccruedInt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("figi")]
        public string Figi { get; set; }

        [JsonProperty("cancelDateTime")]
        public DateTime CancelDateTime { get; set; }

        [JsonProperty("tradesInfo")]
        public TradesInfo TradesInfo { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("yield")]
        public Yield Yield { get; set; }

        [JsonProperty("payment")]
        public Payment Payment { get; set; }

        [JsonProperty("commission")]
        public Commission Commission { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("quantityRest")]
        public long QuantityRest { get; set; }

        [JsonProperty("cancelReason")]
        public string CancelReason { get; set; }

        [JsonProperty("instrumentType")]
        public string InstrumentType { get; set; }

        [JsonProperty("childOperations")]
        public List<ChildOperation> ChildOperations { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("parentOperationId")]
        public string ParentOperationId { get; set; }

        [JsonProperty("positionUid")]
        public string PositionUid { get; set; }

        [JsonProperty("quantityDone")]
        public long QuantityDone { get; set; }

        [JsonProperty("yieldRelative")]
        public YieldRelative YieldRelative { get; set; }

        [JsonProperty("instrumentUid")]
        public string InstrumentUid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Payment : CurrencyPrice
    {

    }

    public class Price : CurrencyPrice
    {

    }

    public class OperationHistory
    {
        [JsonProperty("nextCursor")]
        public string NextCursor { get; set; }

        [JsonProperty("hasNext")]
        public bool HasNext { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public class Trade
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("yieldRelative")]
        public YieldRelative YieldRelative { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("num")]
        public string Num { get; set; }

        [JsonProperty("yield")]
        public Yield Yield { get; set; }
    }

    public class TradesInfo
    {
        [JsonProperty("trades")]
        public List<Trade> Trades { get; set; }
    }

    public class Yield : CurrencyPrice
    {

    }

    public class YieldRelative : PriceBase
    {

    }
}
