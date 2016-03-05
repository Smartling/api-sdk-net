using System;

namespace Smartling.Api.Exceptions
{
  public class MaintenanceModeException : Exception
  {
    public MaintenanceModeException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}