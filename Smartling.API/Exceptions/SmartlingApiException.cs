﻿using System;

namespace Smartling.Api.Exceptions
{
  public class SmartlingApiException : Exception
  {
    public SmartlingApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}
