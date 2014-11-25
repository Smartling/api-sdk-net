using System.Linq;
using System.Net;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smartling.Api;

namespace Smartling.ApiTests
{
  [TestClass]
  public class TranslatorTests
  {
    private const string ResponseString = "{\"response\":{\"data\":{\"fileUri\":\"sample.xml\",\"wordCount\":126,\"fileType\":\"xml\",\"callbackUrl\":null,\"lastUploaded\":\"2013-08-05T17:44:55\",\"stringCount\":4,\"approvedStringCount\":4,\"completedStringCount\":0},\"code\":\"SUCCESS\",\"messages\":[]}}";
    private const string ListFilesResponse = "{\"response\":{\"data\":{\"fileCount\":\"1\", \"fileList\": [{\"fileUri\":\"sample.xml\",\"wordCount\":126,\"fileType\":\"xml\",\"callbackUrl\":null,\"lastUploaded\":\"2013-08-05T17:44:55\",\"stringCount\":4,\"approvedStringCount\":4,\"completedStringCount\":0}]} ,\"code\":\"SUCCESS\",\"messages\":[]}}";
    private const string GetFileResponse = @"<?xml version=""1.0\"" encoding=""utf-8\""?>
                                              <!--smartling.translate_paths = sitecore/phrase/en-->
                                              <!--smartling.string_format_paths = html : sitecore/phrase/en-->
                                              <sitecore>
                                                <phrase path=""/sitecore/content/Home"" key=""Home"" itemid=""{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}"" fieldid=""Title"" updated=""20090618T125323"">
                                                  <en>Sitecore</en>
                                                </phrase>
                                              </sitecore></xml>";

    [TestMethod]
    public void GetFileStatusTest()
    {
      // Arrange
      var translator = new Mock<Translator>("test", "test", "test", null);
      translator.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ResponseString);
      translator.Setup(foo => foo.PrepareGetRequest(string.Empty)).Returns((WebRequest) null);

      // Act
      var fileStatus = translator.Object.GetFileStatus("test", "ru-RU");

      // Assert
      Assert.AreEqual(fileStatus.wordCount, 126);
      Assert.AreEqual(fileStatus.stringCount, 4);
      Assert.AreEqual(fileStatus.fileType, "xml");
      Assert.AreEqual(fileStatus.approvedStringCount, 4);
      Assert.AreEqual(fileStatus.completedStringCount, 0);
    }

    [TestMethod]
    public void ShouldUploadFiles()
    {
      // Arrange
      var translator = new Mock<Translator>("test", "test", "test", null);
      translator.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ResponseString);
      translator.Setup(foo => foo.PreparePostRequest(It.IsAny<string>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var fileStatus = translator.Object.UploadFile(@"C:\Sample.xml", "Home_{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}_en.xml", "xml", "ru-RU", true);

      // Assert
      Assert.AreEqual(fileStatus.wordCount, 126);
      Assert.AreEqual(fileStatus.stringCount, 4);
      Assert.AreEqual(fileStatus.fileType, "xml");
      Assert.AreEqual(fileStatus.approvedStringCount, 4);
      Assert.AreEqual(fileStatus.completedStringCount, 0);
      translator.Verify(t => t.GetResponse(It.IsAny<WebRequest>()));
      translator.Verify(t => t.PreparePostRequest(It.IsAny<string>(), It.IsAny<string>()));
    }

    [TestMethod]
    public void ShouldGetFileContents()
    {
      // Arrange
      var translator = new Mock<Translator>("test", "test", "test", null);
      translator.Setup(x => x.GetFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).CallBase();
      translator.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(GetFileResponse);
      translator.Setup(foo => foo.PreparePostRequest(It.IsAny<string>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var file = translator.Object.GetFile("Home_{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}_en.xml", "ru-RU", "published");

      // Assert
      Assert.AreEqual(GetFileResponse, file);
      translator.Verify(x => x.PrepareGetRequest(It.Is<string>(r => r.Contains("retrievalType=published"))));
      translator.Verify(x => x.PrepareGetRequest(It.Is<string>(r => r.Contains("locale=ru-RU"))));
      translator.Verify(x => x.PrepareGetRequest(It.Is<string>(r => r.Contains("fileUri=Home_{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}_en.xml"))));
    }

    [TestMethod]
    public void ShouldGetFiles()
    {
      // Arrange
      var translator = new Mock<Translator>("test", "test", "test", null);
      translator.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ListFilesResponse);
      translator.Setup(foo => foo.PreparePostRequest(It.IsAny<string>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var files = translator.Object.GetFilesList("ru-RU");

      // Assert
      Assert.AreEqual(1, files.Count());
    }
  }
}
