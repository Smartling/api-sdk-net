using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smartling.Api.Authentication;

namespace Smartling.ApiTests
{
    [TestClass]
    public class UserAuthenticateApiTest
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
        public void UserAuthenticateTest()
        {
            var client = new Mock<AuthApiUser>("test", "test")
            {
                CallBase = true
            };
            client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ValidAuthRespone);
            client.Setup(foo => foo.PrepareJsonPostRequest(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).Returns((WebRequest)null);
            var result = client.Object.Authenticate();

            Assert.AreEqual("{access token}", result.data.accessToken);
            Assert.AreEqual("{refresh token}", result.data.refreshToken);
        }
    }
}
