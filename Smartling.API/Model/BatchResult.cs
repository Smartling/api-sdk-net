using System;
using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class BatchResult
  {
    public string status { get; set; }
    public bool authorized { get; set; }
    public object generalErrors { get; set; }
    public string jobUid { get; set; }
    public string smartlingProjectId { get; set; }
    public DateTime updatedDate { get; set; }
    public List<BatchFile> files { get; set; }
  }

  public class BatchTargetLocale
  {
    public string localeId { get; set; }
    public int stringsAdded { get; set; }
    public int stringsSkipped { get; set; }
  }

  public class BatchFile
  {
    public string status { get; set; }
    public DateTime updatedDate { get; set; }
    public object errors { get; set; }
    public string fileUri { get; set; }
    public List<BatchTargetLocale> targetLocales { get; set; }
  }
}
