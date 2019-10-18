using Newtonsoft.Json;
using System;

namespace Smartling.Api.Model
{
    public class TranslationJob
    {
        [JsonProperty("jobName")]
        public string JobName { get; set; }
        [JsonProperty("translationJobUid")]
        public string TranslationJobUid { get; set; }
        [JsonProperty("jobDueDate")]
        public DateTime? DueDate { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }
        [JsonProperty("jobNumber")]
        public string JobNumber { get; set; }
        [JsonProperty("workflowStepDueDate")]
        public DateTime? WorkflowStepDueDate { get; set; }
    }
}
