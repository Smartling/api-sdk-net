using Newtonsoft.Json;

namespace Smartling.Api.Model
{
    public class Project
    {
        [JsonProperty("projectName")]
        public string ProjectName { get;set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
    }
}
