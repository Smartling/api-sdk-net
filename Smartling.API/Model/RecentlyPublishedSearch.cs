using System;
using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class RecentlyPublishedSearch
  {
    public RecentlyPublishedSearch(DateTime publishedAfter)
    {
      PublishedAfter = publishedAfter;
    }

    public DateTime PublishedAfter { get; set; }
    public List<string> FileUris { get; } = new List<string>();
    public List<string> LocaleIds { get; } = new List<string>();
    public int Limit { get; set; }
    public int Offset { get; set; }
    public string LastError { get; set; }

    public bool IsValid()
    {
      if((DateTime.UtcNow - PublishedAfter).TotalDays > 14)
      {
        LastError = "PublishedAfter is limited to 14 days back from now";
        return false;
      }

      if (FileUris.Count > 20)
      {
        LastError = "FileUris max amount is 20 files";
        return false;
      }

      if (LocaleIds.Count > 50)
      {
        LastError = "LocaleIds max amount is 50 locales";
        return false;
      }

      if(Limit < 0)
      {
        LastError = "Limit must not be negative";
        return false;
      }

      if (Offset < 0)
      {
        LastError = "Offset must not be negative";
        return false;
      }

      if (Offset > 0 && Limit <= 0)
      {
        LastError = "Offset requires Limit > 0";
        return false;
      }

      return true;
    }
  }
}
