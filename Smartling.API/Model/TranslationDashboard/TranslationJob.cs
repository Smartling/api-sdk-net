using System;
using System.Collections.Generic;
using System.Text;

namespace Smartling.Api.Model.TranslationDashboard
{
    public class TranslationJob
    {
        public string jobName { get; set; }
        public string translationJobUid { get; set; }
        public string jobDueDate { get; set; }
        public string description { get; set; }
        public string referenceNumber { get; set; }
        public string jobNumber { get; set; }
        public DateTime? workflowStepDueDate { get; set; }
    }
}
