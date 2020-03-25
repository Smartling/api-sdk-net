using System;

namespace Smartling.Api.Exceptions
{
  public class MaintenanceModeException : SmartlingApiException
  {
    public MaintenanceModeException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}