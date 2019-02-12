using System;
using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class TranslationRequest<TCustomRequest, TCustomSubmission>
  {
    public string translationRequestUid { get; set; }
    public string projectId { get; set; }
    public string bucketName { get; set; }
    public OriginalAssetKey originalAssetKey { get; set; }
    public string title { get; set; }
    public string fileUri { get; set; }
    public int totalWordCount { get; set; }
    public int totalStringCount { get; set; }
    public string contentHash { get; set; }
    public string originalLocale { get; set; }
    public bool outdated { get; set; }
    public DateTime localeLastModifiedDate { get; set; }
    public TCustomRequest customOriginalData { get; set; }
    public DateTime createdDate { get; set; }
    public DateTime modifiedDate { get; set; }
    public List<TranslationSubmission<TCustomSubmission>> translationSubmissions { get; set; }
  }
}