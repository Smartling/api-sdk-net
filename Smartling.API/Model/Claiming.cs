using Newtonsoft.Json;

namespace Smartling.Api.Model
{
    public class Claiming
    {
        [JsonProperty("isClaimable")]
        public bool IsClaimable { get; set; }
        [JsonProperty("isUnclaimable")]
        public bool IsUnclaimable { get; set; }
        [JsonProperty("claimableWordCount")]
        public int ClaimableWordCount { get; set; }
    }
}
