using Newtonsoft.Json;

namespace Buratino.API.Dto
{
    public class Brand
    {
        [JsonProperty("logoName")]
        public string LogoName { get; set; }

        [JsonProperty("logoBaseColor")]
        public string LogoBaseColor { get; set; }

        [JsonProperty("textColor")]
        public string TextColor { get; set; }
    }
}
