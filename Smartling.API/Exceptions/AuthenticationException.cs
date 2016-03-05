using System;

namespace Smartling.Api.Exceptions
{
  public class AuthenticationException : Exception
  {
    public AuthenticationException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}