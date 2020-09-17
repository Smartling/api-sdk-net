using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Smartling.Api.Model;
using Smartling.Api.Threading;

namespace Smartling.Api.CloudLog
{
  public class SmartlingCloudLogger : IDisposable
  {
    private const int BufferMaxSize = 100;
    private const int TimeoutSeconds = 3;
    public enum FlushMode { ForceFlush, DefaultFlush }
    private Timer idleFlushTimer;
    private const int DefaultLogThreads = 5;

    private ConcurrentQueue<LoggingEventData> loggingEvents = new ConcurrentQueue<LoggingEventData>();

    // We share the same thread pool across all instances
    private static readonly LimitedConcurrencyLevelTaskScheduler scheduler = new LimitedConcurrencyLevelTaskScheduler(DefaultLogThreads);
    private static readonly TaskFactory taskFactory = new TaskFactory(scheduler);

    private readonly CloudLogApiClient logApiClient;

    // TODO : Add buffer size and timeout here
    public SmartlingCloudLogger(CloudLogApiClient logApiClient)
    {
      this.logApiClient = logApiClient;
      var idleTimeThreshold = TimeSpan.FromSeconds(TimeoutSeconds);
      idleFlushTimer = new Timer(this.TimedFlush, null, idleTimeThreshold, idleTimeThreshold);
    }

    public void Append(LoggingEventData loggingEventData)
    {
      if (string.IsNullOrEmpty(loggingEventData.Level)) return;

      loggingEvents.Enqueue(loggingEventData);
      Flush(FlushMode.DefaultFlush);
    }

    private void TimedFlush(object state)
    {
      this.Flush(FlushMode.ForceFlush);
    }

    protected internal virtual void Flush(FlushMode flushMode)
    {
      var logEventsCount = loggingEvents.Count;
      if (logEventsCount <= 0)
      {
        return;
      }

      if (logEventsCount <= BufferMaxSize && flushMode == FlushMode.DefaultFlush)
      {
        return;
      }

      var events = Interlocked.Exchange(ref loggingEvents, new ConcurrentQueue<LoggingEventData>());
      SendToCloudAsync(events);
    }

    protected internal virtual void SendToCloudAsync(IProducerConsumerCollection<LoggingEventData> events)
    {
      taskFactory.StartNew(() => logApiClient.SendToCloud(events));
    }

    public void Dispose()
    {
      idleFlushTimer?.Dispose();
    }
  }
}
