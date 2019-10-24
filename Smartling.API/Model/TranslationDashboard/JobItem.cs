namespace Smartling.Api.Model.TranslationDashboard
{
    public class JobItem
    {
        public Account account { get; set; }
        public Project project { get;set;}
        public TranslationJob translationJob { get; set; }
        public string targetLocaleId { get; set; }
        public string originalLocaleId { get; set; }
        public WorkflowStep workflowStep { get; set; }
        public int wordCount { get; set; }
        public string stringCount { get; set; }
        public JobItemIssues issues { get; set; }
        public Claiming claiming { get; set; }
        public bool offlineWorkEnabled { get; set; }
        public object gracefulWindow { get; set; }
        public bool? isActionable { get; set; }
        public bool? isDeclined { get; set; }
        public bool? isContentUpdated { get; set; }
    }    
}
