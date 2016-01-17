using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smartling.Api.Authentication;

namespace Smartling.ApiTests
{
  [TestClass]
  public class OAuthAuthenticationStrategyTests
  {
    private const string ValidAuthRespone = @"{
          ""response"" : {
              ""code"" : ""SUCCESS"",
              ""data"" : {
                  ""accessToken"": ""{access token}"",
                  ""expiresIn"": 1455,
                  ""refreshExpiresIn"": 1455,
                  ""refreshToken"": ""{refresh token}"",
                  ""tokenType"": ""Bearer"",
                  ""sessionState"": """"
              }
      }
      }";

    private const string ExpiredAuthRespone = @"{
          ""response"" : {
              ""code"" : ""SUCCESS"",
              ""data"" : {
                  ""accessToken"": ""{access token}"",
                  ""expiresIn"": 0,
                  ""refreshExpiresIn"": 1455,
                  ""refreshToken"": ""{refresh token}"",
                  ""tokenType"": ""Bearer"",
                  ""sessionState"": """"
              }
      }
      }";
    
    private const string ExpiredRefreshRespone = @"{
          ""response"" : {
              ""code"" : ""SUCCESS"",
              ""data"" : {
                  ""accessToken"": ""{access token}"",
                  ""expiresIn"": 0,
                  ""refreshExpiresIn"": 0,
                  ""refreshToken"": ""{refresh token}"",
                  ""tokenType"": ""Bearer"",
                  ""sessionState"": """"
              }
      }
      }";

    private const string AuthErrorResponse = @"
      {""response"":{  
         ""code"":""VALIDATION_ERROR"",
         ""errors"":[
            {  
               ""key"":null,
               ""message"":""HTTP 401 Unauthorized"",
               ""details"":null
            }
         ]
      }
      }";

    [TestMethod]
    public void GetTokenShouldUseCache()
    {
      // Arrange
      var client = new Mock<AuthApiClient>("test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ValidAuthRespone);
      client.Setup(foo => foo.CreatePostRequest(It.IsAny<string>(), It.IsAny<Object>())).Returns((WebRequest)null);
      var strategy = new OAuthAuthenticationStrategy(client.Object);

      // Act
      var accessToken = strategy.GetToken();
      accessToken = strategy.GetToken();

      // Assert
      client.Verify(x => x.Authenticate(), Times.Once());
      client.Verify(x => x.Refresh(It.IsAny<string>()), Times.Never);
      Assert.AreEqual("{access token}", accessToken);
    }

    [TestMethod]
    public void GetTokenShouldRefreshExpiredToken()
    {
      // Arrange
      var client = new Mock<AuthApiClient>("test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ExpiredAuthRespone);
      client.Setup(foo => foo.CreatePostRequest(It.IsAny<string>(), It.IsAny<Object>())).Returns((WebRequest)null);
      var strategy = new OAuthAuthenticationStrategy(client.Object);

      // Act
      var accessToken = strategy.GetToken();
      accessToken = strategy.GetToken();

      // Assert
      client.Verify(x => x.Authenticate(), Times.Once());
      client.Verify(x => x.Refresh(It.IsAny<string>()), Times.Once);
      Assert.AreEqual("{access token}", accessToken);
    }

    [TestMethod]
    public void GetTokenShouldAuthWhenRefreshTokenExpired()
    {
      // Arrange
      var client = new Mock<AuthApiClient>("test", "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ExpiredRefreshRespone);
      client.Setup(foo => foo.CreatePostRequest(It.IsAny<string>(), It.IsAny<Object>())).Returns((WebRequest)null);
      var strategy = new OAuthAuthenticationStrategy(client.Object);

      // Act
      var accessToken = strategy.GetToken();
      accessToken = strategy.GetToken();

      // Assert
      client.Verify(x => x.Authenticate(), Times.Exactly(2));
      client.Verify(x => x.Refresh(It.IsAny<string>()), Times.Never);
      Assert.AreEqual("{access token}", accessToken);
    }
  }
}
