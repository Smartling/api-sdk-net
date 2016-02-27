using System.Collections.Generic;

namespace Smartling.Api.Project
{
  public class ProjectData
  {
    public string projectId { get; set; }
    public string projectName { get; set; }
    public string accountUid { get; set; }
    public bool archived { get; set; }
    public object projectTypeDisplayValue { get; set; }
    public List<TargetLocale> targetLocales { get; set; }
    public List<object> categories { get; set; }
    public string sourceLocaleId { get; set; }
    public string sourceLocaleDescription { get; set; }
  }
  
  public class TargetLocale
  {
    public string localeId { get; set; }
    public string description { get; set; }
    public bool enabled { get; set; }
  }
}
