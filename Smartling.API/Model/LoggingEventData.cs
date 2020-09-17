using System;

namespace Smartling.Api.Model
{
  public struct LoggingEventData
  {
    /// <summary>
    /// Integration name.
    /// </summary>
    public string Channel;

    /// <summary>
    /// It's useful to know what layer of the connector caused this log record.
    /// In sitecore it can be "core" and "context". Or it can be just logger name
    /// </summary>
    public string RemoteChannel;

    /// <summary>
    /// Smartling project.
    /// </summary>
    public string ProjectId;

    /// <summary>
    /// Level of logging event. Use log4net level names
    /// </summary>
    public string Level;

    /// <summary>
    /// The name of thread
    /// </summary>
    /// <remarks>
    /// <para>
    /// The name of thread in which this logging event was generated
    /// </para>
    /// </remarks>
    public string ThreadName;

    /// <summary>
    /// The time the event was logged
    /// </summary>
    /// <remarks>
    /// <para>
    /// The TimeStamp is stored in the local time zone for this computer.
    /// </para>
    /// </remarks>
    public DateTime TimeStamp;

    /// <summary>
    /// The application supplied message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The application supplied message of logging event.
    /// </para>
    /// </remarks>
    public string Message;

    /// <summary>
    /// The string representation of the exception
    /// </summary>
    /// <remarks>
    /// <para>
    /// The string representation of the exception
    /// </para>
    /// </remarks>
    public string ExceptionString;

    /// <summary>
    /// Integration version. It will be present in every log record
    /// so keep it short
    /// </summary>
    public string ModuleVersion;
  }
}
