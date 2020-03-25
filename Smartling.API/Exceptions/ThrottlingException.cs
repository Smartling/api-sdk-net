using System;

namespace Smartling.Api.Exceptions
{
  public class ThrottlingException : Exception
  {
    public ThrottlingException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}