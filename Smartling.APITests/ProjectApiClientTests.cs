using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smartling.Api.Authentication;
using Smartling.Api.Project;

namespace Smartling.ApiTests
{
  [TestClass]
  public class ProjectApiClientTests
  {
    private const string ProjectDataValidResponse = "{\"response\":{\"code\":\"SUCCESS\",\"data\":{\"projectId\":\"TEST_PROJECT_ID\",\"projectName\":\"TEST_PROJECT_NAME\",\"accountUid\":\"2f23eaae9\",\"archived\":false,\"projectTypeDisplayValue\":null,\"targetLocales\":[{\"localeId\":\"de-DE\",\"description\":\"German (Germany)\",\"enabled\":true},{\"localeId\":\"en-GB\",\"description\":\"English (United Kingdom)\",\"enabled\":true},{\"localeId\":\"es\",\"description\":\"Spanish (International)\",\"enabled\":true},{\"localeId\":\"es-ES\",\"description\":\"Spanish (Spain)\",\"enabled\":true},{\"localeId\":\"fr-FR\",\"description\":\"French (France)\",\"enabled\":true},{\"localeId\":\"it-IT\",\"description\":\"Italian (Italy)\",\"enabled\":true},{\"localeId\":\"ja-JP\",\"description\":\"Japanese\",\"enabled\":true},{\"localeId\":\"nl\",\"description\":\"Dutch (International)\",\"enabled\":true},{\"localeId\":\"nl-NL\",\"description\":\"Dutch (Netherlands)\",\"enabled\":true},{\"localeId\":\"pl-PL\",\"description\":\"Polish (Poland)\",\"enabled\":true},{\"localeId\":\"ru-RU\",\"description\":\"Russian\",\"enabled\":true},{\"localeId\":\"sl-SI\",\"description\":\"Slovenian\",\"enabled\":true},{\"localeId\":\"sv-SE\",\"description\":\"Swedish\",\"enabled\":true},{\"localeId\":\"uk-UA\",\"description\":\"Ukrainian\",\"enabled\":true},{\"localeId\":\"zh-CN\",\"description\":\"Chinese (Simplified)\",\"enabled\":true}],\"categories\":[],\"sourceLocaleId\":\"en\",\"sourceLocaleDescription\":\"English\"}}}";

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
      var client = new Mock<ProjectApiClient>(auth, "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ProjectDataValidResponse);

      // Act
      var data = client.Object.GetProjectData();

      // Assert
      Assert.AreEqual("TEST_PROJECT_NAME", data.projectName);
      Assert.AreEqual("TEST_PROJECT_ID", data.projectId);
      Assert.AreEqual(15, data.targetLocales.Count);
      client.Verify(t => t.GetResponse(It.IsAny<WebRequest>()), Times.Once);
    }
  }
}
