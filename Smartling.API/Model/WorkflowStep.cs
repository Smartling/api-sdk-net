using Newtonsoft.Json;

namespace Smartling.Api.Model
{
    public class WorkflowStep
    {
        [JsonProperty("workflowStepName")]
        public string WorkflowStepName { get; set; }
        [JsonProperty("workflowStepUid")]
        public string WorkflowStepUid { get; set; }
        [JsonProperty("workflowStepType")]
        public string WorkflowStepType { get; set; }
    }
}
