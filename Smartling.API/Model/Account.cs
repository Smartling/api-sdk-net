using Newtonsoft.Json;

namespace Smartling.Api.Model
{
    public class Account
    {
        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("accountUid")]
        public string AccountUid { get; set; }
    }
}
