using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smartling.Api.Authentication;
using Smartling.Api.File;

namespace Smartling.ApiTests
{
  [TestClass]
  public class FileApiClientTests
  {
    private const string UploadFileResponseString = "{\"response\":{\"data\":{\"wordCount\":126,\"lastUploaded\":\"2013-08-05T17:44:55\",\"stringCount\":4},\"code\":\"SUCCESS\",\"messages\":[]}}";
    private const string GetFileResponse = @"<?xml version=""1.0\"" encoding=""utf-8\""?>
                                              <!--smartling.translate_paths = sitecore/phrase/en-->
                                              <!--smartling.string_format_paths = html : sitecore/phrase/en-->
                                              <sitecore>
                                                <phrase path=""/sitecore/content/Home"" key=""Home"" itemid=""{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}"" fieldid=""Title"" updated=""20090618T125323"">
                                                  <en>Sitecore</en>
                                                </phrase>
                                              </sitecore></xml>";

    private const string ValidAuthRespone = @"{
          ""response"" : {
              ""code"" : ""SUCCESS"",
              ""data"" : {
                  ""accessToken"": ""{access token}"",
                  ""expiresIn"": 1458,
                  ""refreshExpiresIn"": 1455,
                  ""refreshToken"": ""{refresh token}"",
                  ""tokenType"": ""Bearer"",
                  ""sessionState"": """"
              }
      }
      }";

    private const string ValidStatusResponse = @"{
       ""response"": {
           ""code"": ""SUCCESS"",
           ""messages"": [],
           ""data"": {
         ""fileUri"" : ""[/myproject/i18n/admin_ui.properties]"",
         ""totalStringCount"" : ""10"",
         ""totalWordCount"" : ""100"",
         ""authorizedStringCount"" : ""1"",
         ""completedStringCount"": ""1"",
         ""excludedStringCount"":  ""1"",
         ""completedWordCount"" : ""1"",
         ""authorizedWordCount"" : ""1"",
         ""excludedWordCount"" : ""1"",
         ""lastUploaded"" : ""2000-01-01T01:01:01Z"",
         ""fileType"" : ""[fileType]"",
         ""parserVersion"" : ""3"",
         ""hasInstructions"" : ""true""}
         }
        }";

    private const string LastModifiedResponse = @"
      {
       ""response"": {
           ""code"": ""SUCCESS"",
           ""messages"": [],
           ""data"": {
           ""totalCount"": ""10"",
           ""items"" : [{ ""localeId"": ""fr-FR"", ""lastModified"": ""2000-01-01T01:01:01Z"" }]        
        }    
        }
      }";

    private const string LastModifiedDetailResponse = @"
      {
       ""response"": {
           ""code"": ""SUCCESS"",
           ""messages"": [],
           ""data"": {
           ""lastModified"": ""2000-01-01T01:01:01Z""                
        }        
        }
      }";
    
    private static OAuthAuthenticationStrategy GetAuth()
    {
      var authClient = new Mock<AuthApiClient>("test", "test");
      var auth = new OAuthAuthenticationStrategy(authClient.Object);
      authClient.CallBase = true;
      authClient.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ValidAuthRespone);
      authClient.Setup(foo => foo.PrepareJsonPostRequest(It.IsAny<string>(), It.IsAny<Object>(), It.IsAny<string>())).Returns((WebRequest)null);
      return auth;
    }

    [TestMethod]
    public void ShouldUploadFiles()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<FileApiClient>(auth, "test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(UploadFileResponseString);
      client.Setup(foo => foo.PrepareFilePostRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<NameValueCollection>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var fileStatus = client.Object.UploadFile(@"C:\Sample.xml", "Home_{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}_en.xml", "xml", "ru-RU", true);

      // Assert
      Assert.AreEqual(fileStatus.wordCount, 126);
      Assert.AreEqual(fileStatus.stringCount, 4);
      client.Verify(t => t.GetResponse(It.IsAny<WebRequest>()), Times.Once);

      var version = Assembly.GetAssembly(typeof (FileApiClient)).GetName().Version.ToString();
      var clientUid = "{\"client\":\"smartling-api-sdk-net\",\"version\":\"" + version + "\"}";
      client.Verify(foo => foo.PrepareFilePostRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.Is<NameValueCollection>(x => x["smartling.client_lib_id"] == clientUid), It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void ShouldGetFileStatus()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<FileApiClient>(auth, "test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ValidStatusResponse);
      client.Setup(foo => foo.PrepareGetRequest(It.IsAny<string>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var fileStatus = client.Object.GetFileStatus("test");

      // Assert
      Assert.AreEqual(fileStatus.totalStringCount, 10);
      Assert.AreEqual(fileStatus.totalWordCount, 100);
    }

    [TestMethod]
    public void ShouldSetUserAgent()
    {
      // Arrange
      var auth = GetAuth();
      var client = new FileApiClient(auth, "test", "test");

      // Act
      var request = (HttpWebRequest)client.PrepareGetRequest("http://smartling.com", "test");

      // Assert
      var version = Assembly.GetAssembly(typeof (FileApiClient)).GetName().Version.ToString();
      var userAgent = string.Format("smartling-api-sdk-net/{0}", version);
      Assert.AreEqual(userAgent, request.UserAgent);
    }

    [TestMethod]
    public void ShouldGetDetailedFileStatus()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<FileApiClient>(auth, "test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ValidStatusResponse);
      client.Setup(foo => foo.PrepareGetRequest(It.IsAny<string>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var fileStatus = client.Object.GetFileStatus("test", "ru-RU");

      // Assert
      Assert.AreEqual(fileStatus.totalStringCount, 10);
      Assert.AreEqual(fileStatus.totalWordCount, 100);
    }

    [TestMethod]
    public void ShouldGetFileContents()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<FileApiClient>(auth, "test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(GetFileResponse);
      client.Setup(foo => foo.PrepareGetRequest(It.IsAny<string>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var file = client.Object.GetFile("Sample.xml", "ru-RU", "published");

      // Assert
      Assert.AreEqual(GetFileResponse, file);
      client.Verify(x => x.PrepareGetRequest(It.Is<string>(r => r.Contains("retrievalType=published")), It.IsAny<string>()));
      client.Verify(x => x.PrepareGetRequest(It.Is<string>(r => r.Contains("fileUri=Sample.xml")), It.IsAny<string>()));
    }

    [TestMethod]
    public void ShouldReturnLastModified()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<FileApiClient>(auth, "test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(LastModifiedResponse);
      client.Setup(foo => foo.PrepareGetRequest(It.IsAny<string>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var lastModified = client.Object.GetLastModified("test");

      // Assert
      Assert.AreEqual(lastModified.totalCount, 10);
      Assert.AreEqual(lastModified.items[0].localeId, "fr-FR");
    }

    [TestMethod]
    public void ShouldReturnLastModifiedDetail()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<FileApiClient>(auth, "test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(LastModifiedDetailResponse);
      client.Setup(foo => foo.PrepareGetRequest(It.IsAny<string>(), It.IsAny<string>())).Returns((WebRequest)null);

      // Act
      var lastModified = client.Object.GetLastModified("test", "fr-FR");

      // Assert
      Assert.AreEqual(lastModified.lastModified, DateTime.Parse("2000-01-01T01:01:01Z").ToUniversalTime());
    }

    [TestMethod]
    public void ShouldEscapeParametersInLastModified()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<FileApiClient>(auth, "<test&project>", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(LastModifiedResponse);
 
      // Act
      var file = client.Object.GetLastModified("D & G.xml");

      // Assert
      client.Verify(x => x.PrepareGetRequest(It.Is<string>(r => r.Contains("D%20%26%20G.xml")), It.IsAny<string>()));
      client.Verify(x => x.PrepareGetRequest(It.Is<string>(r => r.Contains("%3Ctest%26project%3E")), It.IsAny<string>()));
    }
  }
}
