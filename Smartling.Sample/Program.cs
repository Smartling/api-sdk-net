namespace Smartling.ApiSample
{
  using System;

  using Smartling.Api;

  internal class Program
  {
    private static void Main(string[] args)
    {
      string fileUri = "ApiSample_" + Guid.NewGuid();
      Console.WriteLine("Enter API key:");
      var apiKey = Console.ReadLine();

      Console.WriteLine("Enter project ID:");
      var projectId = Console.ReadLine();

      var translator = new Translator("https://api.smartling.com/v1/", apiKey, projectId, string.Empty);
      Upload(translator, fileUri, "xml");
      List(translator, "ru-RU");
      Get(translator, fileUri, "ru-RU");

      Console.WriteLine("Deleting file...");
      translator.DeleteFile(fileUri);

      Console.WriteLine(string.Empty);
      Console.WriteLine("Done. Press 'd' to delete all files before exiting. Press any other key to just exit.");

      var key = Console.ReadKey();
      if (key.KeyChar == 'd')
      {
        var files = translator.GetFilesList("en");
        foreach (var file in files)
        {
          translator.DeleteFile(file.fileUri);
          Console.WriteLine("Deleted file: " + file.fileUri);
        }

        Console.WriteLine("All files were deleted. Press any key to exit.");
        Console.ReadKey();
      }
    }

    private static void List(Translator translator, string locale)
    {
      Console.WriteLine(string.Empty);
      Console.WriteLine("Listing files...");
      var list = translator.GetFilesList(locale);
      foreach (var file in list)
      {
        Console.WriteLine(file.fileUri + " " + file.wordCount + " " + file.stringCount);
      }
    }

    private static void Get(Translator translator, string fileUri, string locale)
    {
      Console.WriteLine(string.Empty);
      Console.WriteLine("Retrieving file...");
      var status = translator.GetFileStatus(fileUri, locale);
      Console.WriteLine(status.fileUri);
    }

    private static void Upload(Translator translator, string fileUri, string fileType)
    {
      Console.WriteLine(string.Empty);
      Console.WriteLine("Uploading file...");
      var status = translator.UploadFile(@"C:\Sample.xml", fileUri, fileType, "ru-RU", true);
      Console.WriteLine(status.fileUri);
    }
  }
}
