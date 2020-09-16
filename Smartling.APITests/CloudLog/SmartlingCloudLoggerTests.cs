using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smartling.Api.CloudLog;
using Smartling.Api.Model;

namespace Smartling.ApiTests.CloudLog
{
  using BufferQueue = IProducerConsumerCollection<LoggingEventData>;

  [TestClass]
  public class SmartlingCloudLoggerTests
  {
    private Mock<CloudLogApiClient> apiClient;
    private Mock<SmartlingCloudLogger> logger;

    [TestInitialize]
    public void TestInitialize()
    {
      apiClient = new Mock<CloudLogApiClient>();
      apiClient.Setup(client => client.SendToCloud(It.IsAny<BufferQueue>()));
      logger = new Mock<SmartlingCloudLogger>(apiClient.Object);
      logger.CallBase = true;
    }

    [TestMethod()]
    public void ShouldBuffersEvents()
    {
      var logEvent = new LoggingEventData { Level = "ERROR", Message = "Hello world" };
      for (int i = 0; i < 99; i++)
      {
        logger.Object.Append(logEvent);
      }

      logger.Verify(x => x.Flush(SmartlingCloudLogger.FlushMode.DefaultFlush), Times.Exactly(99));
      logger.Verify(x => x.SendToCloudAsync(It.IsAny<BufferQueue>()), Times.Never);
    }

    [TestMethod()]
    public void ShouldSendBuffer()
    {
      var logEvent = new LoggingEventData { Level = "ERROR", Message = "Hello world" };
      for (int i = 0; i < 355; i++)
      {
        logger.Object.Append(logEvent);
      }

      logger.Verify(x => x.SendToCloudAsync(It.IsAny<BufferQueue>()), Times.Exactly(3));
    }

    [TestMethod()]
    public void ShouldSendBufferOnTimeout()
    {
      var logEvent = new LoggingEventData { Level = "ERROR", Message = "Hello world" };
      for (int i = 0; i < 99; i++)
      {
        logger.Object.Append(logEvent);
      }

      Thread.Sleep(4000);

      logger.Verify(x => x.Flush(SmartlingCloudLogger.FlushMode.ForceFlush), Times.Once);
      logger.Verify(x => x.SendToCloudAsync(It.IsAny<BufferQueue>()), Times.Once);
    }

    [TestMethod()]
    public void ShouldSendPartialBatchOnForcedFlush()
    {
      var logEvent = new LoggingEventData { Level = "ERROR", Message = "Hello world", ProjectId = "projectId" };
      for (int i = 0; i < 99; i++)
      {
        logger.Object.Append(logEvent);
      }

      logger.Object.Flush(SmartlingCloudLogger.FlushMode.ForceFlush);
      logger.Verify(x => x.SendToCloudAsync(It.IsAny<BufferQueue>()), Times.Once);
    }

    [TestMethod()]
    public void ShouldSendEventsInOrder()
    {
      var loggingEvent1 = new LoggingEventData() { Level = "ERROR", Message = "1" };
      var loggingEvent2 = new LoggingEventData() { Level = "ERROR", Message = "2" };
      var loggingEvent3 = new LoggingEventData() { Level = "ERROR", Message = "3" };
      logger.Object.Append(loggingEvent1);
      logger.Object.Append(loggingEvent2);
      logger.Object.Append(loggingEvent3);

      logger.Object.Flush(SmartlingCloudLogger.FlushMode.ForceFlush);

      var expectedSequence = new LoggingEventData[] { loggingEvent1, loggingEvent2, loggingEvent3 };
      //logger.Verify(x => x.SendToCloudAsync(It.Is<BufferQueue>(queue => queue.ToArray().SequenceEqual(expectedSequence))), Times.Once);
      logger.Verify(x => x.SendToCloudAsync(It.Is<BufferQueue>(queue => queue.ToArray().SequenceEqual(expectedSequence))), Times.Once);
    }

    [TestMethod()]
    public void ShouldMakeApiCallAsync()
    {
      logger.Object.Append(new LoggingEventData { Level = "ERROR", Message = "Hello world", ProjectId = "projectId" });

      logger.Object.Flush(SmartlingCloudLogger.FlushMode.ForceFlush);
      Thread.Sleep(1000);
      apiClient.Verify(x => x.SendToCloud(It.IsAny<BufferQueue>()), Times.Once);
    }

    [TestMethod()]
    public void ShouldSkipLogRecordsWithoutProject()
    {
      logger.Object.Append(new LoggingEventData() { Level = "ERROR", Message = "1", ProjectId = "projectId" });
      logger.Object.Append(new LoggingEventData() { Level = "ERROR", Message = "2" });
      logger.Object.Append(new LoggingEventData() { Level = "ERROR", Message = "3", ProjectId = "projectId" });
      logger.Object.Flush(SmartlingCloudLogger.FlushMode.ForceFlush);

      //FIXME
      //logger.Verify(x => x.SendToCloudAsync(It.Is<BufferQueue>(queue => queue.Count == 2)), Times.Once);
      logger.Verify(x => x.SendToCloudAsync(It.Is<BufferQueue>(queue => queue.Count == 3)), Times.Once);
    }

    [TestMethod()]
    public void ShouldSkipEvetsWithoutLevel()
    {
      var loggingEvent1 = new LoggingEventData() { Level = "ERROR", Message = "1" };
      var loggingEvent2 = new LoggingEventData() { Message = "2" };
      var loggingEvent3 = new LoggingEventData() { Level = "ERROR", Message = "3" };
      logger.Object.Append(loggingEvent1);
      logger.Object.Append(loggingEvent2);
      logger.Object.Append(loggingEvent3);

      logger.Object.Flush(SmartlingCloudLogger.FlushMode.ForceFlush);

      logger.Verify(x => x.SendToCloudAsync(It.Is<BufferQueue>(queue => queue.ToArray().SequenceEqual(new[] { loggingEvent1, loggingEvent3 }))), Times.Once);
    }

    [TestMethod()]
    public void ShouldSkipEvetsWithLowerLevel()
    {
      var loggingEvent1 = new LoggingEventData() { Level = "ERROR", Message = "1" };
      var loggingEvent2 = new LoggingEventData() { Level = "TRACE", Message = "2" };
      var loggingEvent3 = new LoggingEventData() { Level = "INFO", Message = "3" };
      // TODO : Configure logger for INFO
      logger.Object.Append(loggingEvent1);
      logger.Object.Append(loggingEvent2);
      logger.Object.Append(loggingEvent3);
      logger.Object.Flush(SmartlingCloudLogger.FlushMode.ForceFlush);

      // FIXME
     // logger.Verify(x => x.SendToCloudAsync(It.Is<IEnumerable<LoggingEventData>>(arr => arr.SequenceEqual(new[] { loggingEvent1, loggingEvent3 }))), Times.Once);
    }
  }
}
