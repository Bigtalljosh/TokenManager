using Newtonsoft.Json;

namespace TokenManager
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int Expires { get; set; }
        [JsonProperty("token_type")]
        public string Type { get; set; }
    }
}
