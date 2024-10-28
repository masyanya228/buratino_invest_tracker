using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Nominal
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("units")]
        public decimal Units { get; set; }

        [JsonProperty("nano")]
        public int Nano { get; set; }

        public decimal GetInRub()
        {
            if (Currency == "rub")
                return Units;
            if (Currency == "usd")
                return Units * 97.29m;
            throw new ArgumentOutOfRangeException();
        }
    }
}
