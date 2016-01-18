using System.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.File;

namespace Smartling.ApiSample
{
  using System;
  
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
      fileApiClient.ApiGatewayUrl = "https://api.smartling.com";
      string fileUri = "ApiSample_" + Guid.NewGuid();

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
