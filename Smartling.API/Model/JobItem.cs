using Newtonsoft.Json;

namespace Smartling.Api.Model
{
    public class JobItem
    {
        [JsonProperty("account")]
        public Account Account { get; set; }
        [JsonProperty("project")]
        public Project Project { get;set;}
        [JsonProperty("translationJob")]
        public TranslationJob TranslationJob { get; set; }
        [JsonProperty("targetLocaleId")]
        public string TargetLocaleId { get; set; }
        [JsonProperty("originalLocaleId")]
        public string OriginalLocaleId { get; set; }
        [JsonProperty("workflowStep")]
        public WorkflowStep WorkflowStep { get; set; }
        [JsonProperty("wordCount")]
        public int WordCount { get; set; }
        [JsonProperty("stringCount")]
        public string StringCount { get; set; }
        [JsonProperty("issues")]
        public Issues Issues { get; set; }
        [JsonProperty("claiming")]
        public Claiming Claiming { get; set; }
        [JsonProperty("offlineWorkEnabled")]
        public bool OfflineWorkEnabled { get; set; }
        [JsonProperty("gracefulWindow")]
        public object GracefulWindow { get; set; }
        [JsonProperty("isActionable")]
        public bool? IsActionable { get; set; }
        [JsonProperty("isDeclined")]
        public bool? IsDeclined { get; set; }
        [JsonProperty("isContentUpdated")]
        public bool? IsContentUpdated { get; set; }
    }

    public class Issues
    {
        [JsonProperty("issuesCount")]
        public int IssuesCount { get; set; }
    }

    public class DashboardData
    {
        [JsonProperty("items")]
        public JobItem[] Items { get; set; }
    }
}
