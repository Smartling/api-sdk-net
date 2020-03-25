using System;

namespace Smartling.Api.Exceptions
{
  public class AuthenticationException : SmartlingApiException
  {
    public AuthenticationException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}