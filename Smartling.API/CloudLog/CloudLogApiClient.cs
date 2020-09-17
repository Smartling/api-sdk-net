using Newtonsoft.Json;
using Smartling.Api.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Smartling.Api.CloudLog
{
  public class CloudLogApiClient : ApiClientBase
  {
    private const string PostStatusUrl = "https://api.smartling.com/updates/status";
    private const string JsonContentType = "application/json";
    private readonly string hostName = Environment.MachineName;

    public virtual void SendToCloud(IProducerConsumerCollection<LoggingEventData> events)
    {
      if (events == null)
      {
        return;
      }

      try
      {
        var command = new LogRecords
        {
          records = new List<LogRecord>()
        };

        while (events.TryTake(out var loggingEventData))
        {
          var threadName = string.IsNullOrEmpty(loggingEventData.ThreadName) ? string.Empty : "[" + loggingEventData.ThreadName + "] ";
          command.records.Add(new LogRecord
          {
            channel = loggingEventData.Channel,
            context = new LogContext {
              host = hostName,
              projectId = loggingEventData.ProjectId,
              remoteChannel = loggingEventData.RemoteChannel,
              moduleVersion = loggingEventData.ModuleVersion
            },
            datetime = loggingEventData.TimeStampUtc,
            level_name = loggingEventData.Level,
            message = string.Format("{0}{1} {2}", threadName,
              loggingEventData.Message, loggingEventData.ExceptionString)
          });
        }

        var request = PrepareJsonPostRequest(PostStatusUrl, command);
        GetResponse(request);
      }
      catch
      {
        // ignored
      }
    }

    // FIXME : This method duplicates another method in base class.
    // This one has only one difference. This line uses another const value
    // request.ContentType = JsonContentType;
    private WebRequest PrepareJsonPostRequest(string url, object command)
    {
      var request = (HttpWebRequest)WebRequest.Create(url);
      var json = command is string commandStr
          ? commandStr
          : JsonConvert.SerializeObject(command);

      var postBytes = Encoding.UTF8.GetBytes(json);

      request.Method = WebRequestMethods.Http.Post;
      request.ContentType = JsonContentType;
      request.Accept = JsonContentType;
      request.ContentLength = postBytes.Length;
      request.UserAgent = ApiClientUid.ToUserAgent();

      var requestStream = request.GetRequestStream();
      requestStream.Write(postBytes, 0, postBytes.Length);
      requestStream.Close();
      return request;
    }
  }

  internal struct LogRecords
  {
    public List<LogRecord> records;
  }

  internal struct LogRecord
  {
    public string level_name;
    public string channel;
    [JsonConverter(typeof(Newtonsoft.Json.Converters.IsoDateTimeConverter))]
    public DateTime datetime;
    public LogContext context;
    public string message;
  }

  internal struct LogContext
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string projectId;
    public string host;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string remoteChannel;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string moduleVersion;
  }
}
