using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public partial class Instrument
    {
        [JsonProperty("figi")]
        public string Figi { get; set; }

        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("classCode")]
        public string ClassCode { get; set; }

        [JsonProperty("isin")]
        public string Isin { get; set; }

        [JsonProperty("lot")]
        public int Lot { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("shortEnabledFlag")]
        public bool ShortEnabledFlag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("couponQuantityPerYear")]
        public int CouponQuantityPerYear { get; set; }

        [JsonProperty("maturityDate")]
        public DateTime MaturityDate { get; set; }

        [JsonProperty("nominal")]
        public Nominal Nominal { get; set; }

        [JsonProperty("initialNominal")]
        public InitialNominal InitialNominal { get; set; }

        [JsonProperty("stateRegDate")]
        public DateTime StateRegDate { get; set; }

        [JsonProperty("placementDate")]
        public DateTime PlacementDate { get; set; }

        [JsonProperty("placementPrice")]
        public PlacementPrice PlacementPrice { get; set; }

        [JsonProperty("aciValue")]
        public AciValue AciValue { get; set; }

        [JsonProperty("countryOfRisk")]
        public string CountryOfRisk { get; set; }

        [JsonProperty("countryOfRiskName")]
        public string CountryOfRiskName { get; set; }

        [JsonProperty("sector")]
        public string Sector { get; set; }

        [JsonProperty("issueKind")]
        public string IssueKind { get; set; }

        [JsonProperty("issueSize")]
        public string IssueSize { get; set; }

        [JsonProperty("issueSizePlan")]
        public string IssueSizePlan { get; set; }

        [JsonProperty("tradingStatus")]
        public string TradingStatus { get; set; }

        [JsonProperty("otcFlag")]
        public bool OtcFlag { get; set; }

        [JsonProperty("buyAvailableFlag")]
        public bool BuyAvailableFlag { get; set; }

        [JsonProperty("sellAvailableFlag")]
        public bool SellAvailableFlag { get; set; }

        [JsonProperty("floatingCouponFlag")]
        public bool FloatingCouponFlag { get; set; }

        [JsonProperty("perpetualFlag")]
        public bool PerpetualFlag { get; set; }

        [JsonProperty("amortizationFlag")]
        public bool AmortizationFlag { get; set; }

        [JsonProperty("minPriceIncrement")]
        public MinPriceIncrement MinPriceIncrement { get; set; }

        [JsonProperty("apiTradeAvailableFlag")]
        public bool ApiTradeAvailableFlag { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("realExchange")]
        public string RealExchange { get; set; }

        [JsonProperty("positionUid")]
        public string PositionUid { get; set; }

        [JsonProperty("assetUid")]
        public string AssetUid { get; set; }

        [JsonProperty("forIisFlag")]
        public bool ForIisFlag { get; set; }

        [JsonProperty("forQualInvestorFlag")]
        public bool ForQualInvestorFlag { get; set; }

        [JsonProperty("weekendFlag")]
        public bool WeekendFlag { get; set; }

        [JsonProperty("blockedTcaFlag")]
        public bool BlockedTcaFlag { get; set; }

        [JsonProperty("subordinatedFlag")]
        public bool SubordinatedFlag { get; set; }

        [JsonProperty("liquidityFlag")]
        public bool LiquidityFlag { get; set; }

        [JsonProperty("first1minCandleDate")]
        public DateTime First1minCandleDate { get; set; }

        [JsonProperty("first1dayCandleDate")]
        public DateTime First1dayCandleDate { get; set; }

        [JsonProperty("riskLevel")]
        public string RiskLevel { get; set; }

        [JsonProperty("brand")]
        public Brand Brand { get; set; }

        [JsonProperty("bondType")]
        public string BondType { get; set; }

        [JsonProperty("callDate")]
        public DateTime? CallDate { get; set; }
    }
}
