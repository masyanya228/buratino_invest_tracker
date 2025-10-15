using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class GetAccounts
    {
        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }
    }
}
