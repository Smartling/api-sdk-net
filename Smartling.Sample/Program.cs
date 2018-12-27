using System.Collections.Generic;
using System.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Batch;
using Smartling.Api.File;
using Smartling.Api.Job;
using Smartling.Api.Model;
using Smartling.Api.Project;

namespace Smartling.ApiSample
{
  using Smartling.Api.Extensions;
  using System;
  using static Smartling.Api.Model.AuditLog;

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
      string fileUri = "ApiSample_" + Guid.NewGuid();

      Audit(auth);
      Published(auth);
      Jobs(auth);
      GetProjectData(projectApiClient);
      Upload(fileApiClient, fileUri, "xml");
      List(fileApiClient);
      Status(fileApiClient, fileUri, "ru-RU");
      Download(fileApiClient, fileUri);
      LastModified(fileApiClient, fileUri);
      Authorization(fileApiClient);
      Deletion(fileApiClient, fileUri);

      Console.WriteLine("All done, press any key to exit");
      Console.ReadKey();
    }

    private static void Audit(OAuthAuthenticationStrategy auth)
    {
      var client = new AuditApiClient(auth, projectId);
      client.Create(new AuditLogBuilder("UPLOAD", "testuser", Guid.NewGuid().ToString(), "test_uri", "/sitecore/content").WithJob("test_job").WithSourceVersion(1).WithSourceLocale("en"));

      var query = new Dictionary<string, string>();
      query.Add(string.Empty, "3E241E52-3767-464D-B994-A776C8A060C3");

      var logs = client.Get(null, "_id:desc");
    }

    private static void Published(OAuthAuthenticationStrategy auth)
    {
      var publishedClient = new PublishedFilesApiClient(auth, projectId);

      foreach (var item in publishedClient.GetRecentlyPublished())
      {
        Console.WriteLine(item.fileUri + " " + item.localeId + " " + item.publishDate);
      }
    }

    private static void Jobs(OAuthAuthenticationStrategy auth)
    {
      var jobApiClient = new JobApiClient(auth, projectId);

      foreach(var job in jobApiClient.Get("ApiSample"))
      {
        Console.WriteLine(job.jobName);
      }
      
      var jobRequest = new CreateJob();
      jobRequest.jobName = "ApiSample_Job_" + Guid.NewGuid();
      jobRequest.description = "test";
      jobRequest.dueDate = "2018-11-21T11:51:17Z";
      jobRequest.targetLocaleIds = new List<string>() {"ru-RU"};
      jobRequest.callbackUrl = "https://www.callback.com/smartling/job";
      jobRequest.callbackMethod = "GET";
      jobRequest.referenceNumber = "test";

      jobApiClient.Create(jobRequest);
      var jobs = jobApiClient.Get();
      var processes = jobApiClient.GetProcesses(jobs.First().translationJobUid);

      var updateJob = new UpdateJob();
      updateJob.jobName = jobRequest.jobName;
      updateJob.description = "test2";
      updateJob.dueDate = "2018-11-21T11:51:17Z";
      jobApiClient.Update(updateJob, jobs.First().translationJobUid);
      jobs = jobApiClient.Get();
      jobApiClient.AddLocale("nl-NL", jobs.First().translationJobUid);

      var batchApiClient = new BatchApiClient(auth, projectId, String.Empty);
      var batch =
        batchApiClient.Create(new CreateBatch()
        {
          authorize = true,
          translationJobUid = jobs.First().translationJobUid
        });

      string fileUri = "ApiSample_" + Guid.NewGuid();
      batchApiClient.UploadFile(@"C:\Sample.xml", fileUri, "xml", "ru-RU", true, batch.batchUid);
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
    private static void Upload(FileApiClient client, string fileUri, string fileType)
    {
      Console.WriteLine(string.Empty);
      Console.WriteLine("Uploading file...");

      var status = client.UploadFile(@"C:\Sample.xml", fileUri, fileType, "ru-RU,fr-FR", true);
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
