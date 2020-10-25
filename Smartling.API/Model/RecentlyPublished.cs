using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class RecentlyPublished
  {
    public List<PublishedItem> items { get; set; }
    public int totalCount { get; set; }
  }
}
