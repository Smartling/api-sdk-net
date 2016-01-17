using System;

namespace Smartling.Api.Exceptions
{
  public class AuthorizationException : Exception
  {
    public AuthorizationException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}