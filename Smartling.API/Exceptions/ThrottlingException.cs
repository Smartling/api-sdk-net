using System;

namespace Smartling.Api.Exceptions
{
  public class ThrottlingException : SmartlingApiException
  {
    public ThrottlingException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}