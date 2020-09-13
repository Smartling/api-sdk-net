using System.Collections.Generic;
using System;
using System.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Batch;
using Smartling.Api.File;
using Smartling.Api.Job;
using Smartling.Api.Model;
using Smartling.Api.Project;
using System.Threading;
using Smartling.Api.CloudLog;

namespace Smartling.ApiSample
{
  internal class Program
  {
    private static string userIdentifier;
    private static string userSecret;
    private static string projectId;

    private static void Main(string[] args)
    {
      GetCredentials();
      var authApiClient = new AuthApiClient(userIdentifier, userSecret);
      var auth = new OAuthAuthenticationStrategy(authApiClient);
      var fileApiClient = new FileApiClient(auth, projectId, string.Empty);
      var projectApiClient = new ProjectApiClient(auth, projectId);
      fileApiClient.ApiGatewayUrl = "https://api.smartling.com";
      string fileName = "ApiSample_" + Guid.NewGuid();
      string fileUri = "/master/" + fileName;

      Upload(fileApiClient, fileUri, fileName, "xml");
      Download(fileApiClient, fileUri);
      Submissions(auth);
      Audit(auth);
      Jobs(auth);
      Published(auth);
      GetProjectData(projectApiClient);
      List(fileApiClient);
      Status(fileApiClient, fileUri, "ru-RU");
      LastModified(fileApiClient, fileUri);
      Authorization(fileApiClient);
      Deletion(fileApiClient, fileUri);
      CloudLog();

      Console.WriteLine("All done, press any key to exit");
      Console.ReadKey();
    }

    private static void CloudLog()
    {
      var logger = new SmartlingCloudLogger();

      logger.Append(new LoggingEventData {
        Channel = "ApiSample",
        RemoteChannel = "Smartling.ApiSample.Program",
        ProjectId = "aabbccdd",
        Level = "Info",
        TimeStamp = DateTime.Now,
        ThreadName = "Test thread name",
        Message = "Test message"
      });
      // Logger send async
      Thread.Sleep(4 * 1000);
    }

    private static void Audit(OAuthAuthenticationStrategy auth)
    {
      var client = new AuditApiClient<SampleAuditLog>(auth, projectId);
      var itemId = Guid.NewGuid().ToString();
      SampleAuditLog log = new SampleAuditLogBuilder("sandbox2", "UPLOAD", "testuser", itemId, "test_uri", "/sitecore/content")
        .WithJob("test_job", "aabbcc", "batch1")
        .WithSourceVersion(1)
        .WithSourceLocale("en")
        .WithTargetLocale("ru-RU");
      
      client.Create(log);

      // Wait for audit log to be created and processed
      Thread.Sleep(1000);

      var query = new Dictionary<string, string>();
      query.Add("clientData.ItemId|clientData.Path", itemId);
      query.Add("sourceLocaleId", "en|ru");
      query.Add("envId", "sandbox2");

      var logs = client.Get(query, "_id:desc");
    }

    private static void Published(OAuthAuthenticationStrategy auth)
    {
      var publishedClient = new PublishedFilesApiClient(auth, projectId);

      foreach (var item in publishedClient.GetRecentlyPublished())
      {
        Console.WriteLine(item.fileUri + " " + item.localeId + " " + item.publishDate);
      }
    }

    private static void Submissions(OAuthAuthenticationStrategy auth)
    {
      var client = new SubmissionApiClient<SampleOriginalAssetKey, SampleCustomTranslationRequestData, SampleTargetAssetKey, SampleCustomSubmissionData>(auth, projectId, "1e65ee5c-2555-4fd4-8305-56228ee3a0dd");

      var itemId = Guid.NewGuid().ToString();

      // List translation requests
      var query = new Dictionary<string, string>();
      query.Add("state", "In Progress|Translated|Completed");
      foreach (var s in client.GetPage(query, 100, 0).items)
      {
        Console.WriteLine(s.translationRequestUid + " " + s.translationSubmissions.Count() + " " + s.fileUri);
        foreach (var sub in s.translationSubmissions)
        {
          Console.WriteLine("  " + sub.state + sub.targetLocaleId);
        }
      }

      // Sample code to process items in bulk
      foreach (var item in client.GetAll(string.Empty, string.Empty))
      {
        var r = new UpdateTranslationRequest<SampleOriginalAssetKey, SampleCustomTranslationRequestData, SampleTargetAssetKey, SampleCustomSubmissionData>();
        if (item.translationSubmissions == null || item.translationSubmissions.Where(x => x.state != "Deleted").Count() == 0)
        {
          continue;
        }

        r.translationSubmissions = new List<UpdateSubmissionRequest<SampleTargetAssetKey, SampleCustomSubmissionData>>();
        foreach (TranslationSubmission<SampleTargetAssetKey,  SampleCustomSubmissionData> s in item.translationSubmissions)
        {
          r.translationSubmissions.Add(new UpdateSubmissionRequest<SampleTargetAssetKey, SampleCustomSubmissionData>
          {
            translationSubmissionUid = s.translationSubmissionUid,
            state = "Deleted"
          });
        }

        var ur = client.UpdateTranslationRequest(r, item.translationRequestUid);
      }

      var singleRequest = client.Get("27c4b81d8d52");

      // Create translation request
      var createTranslationRequest = new CreateTranslationRequest<SampleOriginalAssetKey, SampleCustomTranslationRequestData>();
      createTranslationRequest.contentHash = Guid.NewGuid().ToString().Substring(0, 32);
      createTranslationRequest.fileUri = Guid.NewGuid().ToString();
      createTranslationRequest.originalAssetKey = new SampleOriginalAssetKey() { Key = itemId };
      createTranslationRequest.originalLocaleId = "en";
      createTranslationRequest.title = "test";
      createTranslationRequest.customOriginalData = new SampleCustomTranslationRequestData() { ItemId = itemId, Path = "content/home" };

      var request = client.CreateTranslationRequest(createTranslationRequest);

      // Search submissions
      var searchResult = client.GetPage("originalAssetKey.Key", itemId, 100, 0);
      searchResult = client.GetPage("translationRequestUid", "2e3b50ec4de3", 100, 0);

      query = new Dictionary<string, string>();
      query.Add("state", "Translated|Completed");
      query.Add("customTranslationData", "{\"MediaContent\": false }");
      // query.Add("customOriginalData", "{\"Path\": \"/sitecore/content/Home/Team/Chris-Castle\" }");
      searchResult = client.GetPage(query, 100, 0);

      // Create subsmission
      var submission = new CreateSubmissionRequest<SampleTargetAssetKey, SampleCustomSubmissionData>();
      submission.state = "Deleted";
      submission.submitterName = "test";
      submission.targetLocaleId = "ru-RU";
      submission.targetAssetKey = new SampleTargetAssetKey() { Key = Guid.NewGuid().ToString() };
      submission.customTranslationData = new SampleCustomSubmissionData() { Revision = Guid.NewGuid().ToString(), Locked = false, MediaContent = false };

      request = client.CreateSubmission(request.translationRequestUid, new List<CreateSubmissionRequest<SampleTargetAssetKey, SampleCustomSubmissionData>>() { submission });

      // Update submission
      var updateRequest = new UpdateTranslationRequest<SampleOriginalAssetKey, SampleCustomTranslationRequestData, SampleTargetAssetKey, SampleCustomSubmissionData>();
      updateRequest.customOriginalData = request.customOriginalData;
      updateRequest.customOriginalData.Path = "newpath";
      updateRequest.translationSubmissions = new List<UpdateSubmissionRequest<SampleTargetAssetKey, SampleCustomSubmissionData>> {new UpdateSubmissionRequest<SampleTargetAssetKey, SampleCustomSubmissionData> {
        translationSubmissionUid = request.translationSubmissions[0].translationSubmissionUid,
        state = "In Progress",
        lastExportedDate = DateTime.UtcNow
      }  };

      var updatedRequest = client.UpdateTranslationRequest(updateRequest, request.translationRequestUid);

      // List translation requests
      foreach (var s in client.GetAll("state", "In Progress"))
      {
        Console.WriteLine(s.translationRequestUid + " " + s.translationSubmissions.Count() + " " + s.fileUri);
      }
    }

    private static void Jobs(OAuthAuthenticationStrategy auth)
    {
      var jobApiClient = new JobApiClient(auth, projectId);

      foreach(var job in jobApiClient.Get("ApiSample", new List<string>() { "AWAITING_AUTHORIZATION", "IN_PROGRESS", "IN_PROGRESS" }))
      {
        Console.WriteLine(job.jobName);
      }
      
      var jobRequest = new CreateJob();
      jobRequest.jobName = "ApiSample_Job_" + Guid.NewGuid();
      jobRequest.description = "test";
      jobRequest.dueDate = DateTime.Now.AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ssZ");
      jobRequest.targetLocaleIds = new List<string>() {"ru-RU"};
      jobRequest.callbackUrl = "https://www.callback.com/smartling/job";
      jobRequest.callbackMethod = "GET";
      jobRequest.referenceNumber = "test";

      var createdJob = jobApiClient.Create(jobRequest);
      createdJob = jobApiClient.GetById(createdJob.translationJobUid);

      var jobs = jobApiClient.Get();
      var processes = jobApiClient.GetProcesses(jobs.First().translationJobUid);

      var updateJob = new UpdateJob();
      updateJob.jobName = jobRequest.jobName;
      updateJob.description = "test2";
      updateJob.dueDate = DateTime.Now.AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ssZ");
      var updatedJob = jobApiClient.Update(updateJob, jobs.First().translationJobUid);
      jobs = jobApiClient.Get();
      jobApiClient.AddLocale("nl-NL", jobs.First().translationJobUid);

      var batchApiClient = new BatchApiClient(auth, projectId, String.Empty);
      var batch =
        batchApiClient.Create(new CreateBatch()
        {
          authorize = true,
          translationJobUid = jobs.First().translationJobUid
        });

      string filePath = "ApiSample_" + Guid.NewGuid();
      string fileUri = "/master/" + filePath;
      batchApiClient.UploadFile(@"C:\Sample.xml", fileUri, "xml", "ru-RU", true, batch.batchUid, filePath);
      batchApiClient.Execute(batch.batchUid);

      var batchResult = batchApiClient.Get(batch.batchUid);
    }

    private static void GetProjectData(ProjectApiClient projectApiClient)
    {
      var data = projectApiClient.GetProjectData();
      Console.WriteLine("Project Name: " + data.projectName);
      Console.WriteLine("Locales: ");
      foreach (var targetLocale in data.targetLocales)
      {
        Console.WriteLine(targetLocale.localeId + " " + targetLocale.enabled);
      }
    }

    private static void Deletion(FileApiClient fileClient, string fileUri)
    {
      Console.WriteLine("Deleting file...");
      fileClient.DeleteFile(fileUri);
    }

    private static void Authorization(FileApiClient fileClient)
    {
      fileClient.Authorize("Sample.xml", "de-DE,fr-FR");
      fileClient.Unauthorize("Sample.xml", "fr-FR");
    }

    private static void LastModified(FileApiClient fileClient, string fileUri)
    {
      var lastModified = fileClient.GetLastModified(fileUri);
      var lastModifiedDetail = fileClient.GetLastModified(fileUri, "ru-RU");
      Console.WriteLine("Last modified...");
      Console.WriteLine(lastModified.totalCount);
      Console.WriteLine(lastModifiedDetail.lastModified);
    }

    private static void Download(FileApiClient fileClient, string fileUri)
    {
      Console.WriteLine("Download...");
      var downloadedFile = fileClient.GetFile(fileUri, "ru-RU", "pseudo");
      var fileStream = fileClient.GetFileStream(fileUri, "ru-RU");
      Console.WriteLine(downloadedFile);
    }

    private static void GetCredentials()
    {
      userIdentifier = Environment.GetEnvironmentVariable("userIdentifier");
      userSecret = Environment.GetEnvironmentVariable("userSecret");
      projectId = Environment.GetEnvironmentVariable("projectId");

      if (string.IsNullOrEmpty(userIdentifier))
      {
        Console.WriteLine("Enter userIdentifier:");
        userIdentifier = Console.ReadLine();
      }

      if (string.IsNullOrEmpty(userSecret))
      {
        Console.WriteLine("Enter userSecret:");
        userSecret = Console.ReadLine();
      }

      if (string.IsNullOrEmpty(projectId))
      {
        Console.WriteLine("Enter projectId:");
        projectId = Console.ReadLine();
      }
    }

    private static void List(FileApiClient client)
    {
      Console.WriteLine(string.Empty);
      Console.WriteLine("Listing files...");
      var list = client.GetFilesList().ToList();
      foreach (var file in list)
      {
        Console.WriteLine(file.fileUri + " " + file.hasInstructions + " " + file.lastUploaded);
      }

      foreach (var file in list.Take(10))
      {
        Status(client, file.fileUri, "ru-RU");
      }
    }

    private static void Status(FileApiClient client, string fileUri, string locale)
    {
      Console.WriteLine(string.Empty);
      Console.WriteLine("Retrieving file...");
      var status = client.GetFileStatus(fileUri, locale);
      var fileStatusDetail = client.GetFileStatus(fileUri, "ru-RU");

      Console.WriteLine(status.fileUri);
      Console.WriteLine(fileStatusDetail.totalWordCount);
    }

    /// <summary>
    /// It is also possible to upload file content directly: 
    /// var fileContents = File.ReadAllText(@"C:\Sample.xml");
    /// var status = client.UploadFileContents(fileContents, fileUri, fileType, "ru-RU,fr-FR", true);
    /// </summary>
    /// <param name="client"></param>
    /// <param name="fileUri"></param>
    /// <param name="fileType"></param>
    private static void Upload(FileApiClient client, string fileUri, string filePath, string fileType)
    {
      Console.WriteLine(string.Empty);
      Console.WriteLine("Uploading file...");

      var status = client.UploadFile(@"C:\Sample.xml", fileUri, fileType, "ru-RU,es", true, filePath);
      Console.WriteLine(status.stringCount);
    }

    private void DeleteAllFiles(FileApiClient client)
    {
      Console.WriteLine("Done. Press 'd' to delete all files before exiting. Press any other key to just exit.");

      var key = Console.ReadKey();
      if (key.KeyChar == 'd')
      {
        var files = client.GetFilesList();
        foreach (var file in files)
        {
          client.DeleteFile(file.fileUri);
          Console.WriteLine("Deleted file: " + file.fileUri);
        }

        Console.WriteLine("All files were deleted. Press any key to exit.");
        Console.ReadKey();
      }
    }
  }
}

