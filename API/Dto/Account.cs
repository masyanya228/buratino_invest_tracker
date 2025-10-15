using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Account
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("openedDate")]
        public DateTime OpenedDate { get; set; }

        [JsonProperty("closedDate")]
        public DateTime ClosedDate { get; set; }

        [JsonProperty("accessLevel")]
        public string AccessLevel { get; set; }
    }
}
