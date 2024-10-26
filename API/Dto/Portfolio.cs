using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Portfolio
    {
        [JsonProperty("totalAmountShares")]
        public TotalAmountShares TotalAmountShares { get; set; }

        [JsonProperty("totalAmountBonds")]
        public TotalAmountBonds TotalAmountBonds { get; set; }

        [JsonProperty("totalAmountEtf")]
        public TotalAmountEtf TotalAmountEtf { get; set; }

        [JsonProperty("totalAmountCurrencies")]
        public TotalAmountCurrencies TotalAmountCurrencies { get; set; }

        [JsonProperty("totalAmountFutures")]
        public TotalAmountFutures TotalAmountFutures { get; set; }

        [JsonProperty("expectedYield")]
        public ExpectedYield ExpectedYield { get; set; }

        [JsonProperty("positions")]
        public List<Position> Positions { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("totalAmountOptions")]
        public TotalAmountOptions TotalAmountOptions { get; set; }

        [JsonProperty("totalAmountSp")]
        public TotalAmountSp TotalAmountSp { get; set; }

        [JsonProperty("totalAmountPortfolio")]
        public TotalAmountPortfolio TotalAmountPortfolio { get; set; }

        [JsonProperty("virtualPositions")]
        public List<object> VirtualPositions { get; set; }
    }
}
