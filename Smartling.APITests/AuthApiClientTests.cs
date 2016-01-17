using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smartling.Api.Authentication;

namespace Smartling.ApiTests
{
  [TestClass]
  public class AuthApiClientTests
  {
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

    [TestMethod]
    public void AuthenticateTest()
    {
      // Arrange
      var client = new Mock<AuthApiClient>("test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ValidAuthRespone);
      client.Setup(foo => foo.PrepareJsonPostRequest(It.IsAny<string>(), It.IsAny<Object>())).Returns((WebRequest) null);

      // Act
      var result = client.Object.Authenticate();

      // Assert
      Assert.AreEqual("{access token}", result.data.accessToken);
      Assert.AreEqual("{refresh token}", result.data.refreshToken);
    }

    [TestMethod]
    public void RefreshTest()
    {
      // Arrange
      var client = new Mock<AuthApiClient>("test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ValidAuthRespone);
      client.Setup(foo => foo.PrepareJsonPostRequest(It.IsAny<string>(), It.IsAny<Object>())).Returns((WebRequest) null);

      // Act
      var result = client.Object.Refresh("test");

      // Assert
      Assert.AreEqual("{access token}", result.data.accessToken);
      Assert.AreEqual("{refresh token}", result.data.refreshToken);
    }
  }
}