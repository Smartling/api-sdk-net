namespace Smartling.Api.Model
{
  public class ErrorItem
  {
    public string message;
    public ErrorDetail details;
  }

  public class ErrorDetail
  {
    public string field;

    public override string ToString()
    {
      return "Field: " + field;
    }
  }
}
