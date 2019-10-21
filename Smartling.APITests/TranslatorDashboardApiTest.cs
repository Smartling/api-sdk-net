using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smartling.Api.Authentication;
using Smartling.Api.TranslatorDashboardApiClient;
using System;
using System.Linq;
using System.Net;

namespace Smartling.ApiTests
{
    [TestClass]
    public class TranslatorDashboardApiTest
    {
        private const string DashboardValidationResponse = @"{""response"": {
        ""code"": ""SUCCESS"",
        ""data"": {
            ""items"": [
                {
                    ""account"": {
                        ""accountName"": ""{accountName}"",
                        ""accountUid"": ""{accountUid}""
                    },
                    ""project"": {
                        ""projectName"": ""TEST_PROJECT_NAME"",
                        ""projectId"": ""TEST_PROJECT_ID""
                    },
                    ""translationJob"": {
                        ""jobName"": ""{jobName}"",
                        ""translationJobUid"": ""{translationJobUid}"",
                        ""jobDueDate"": ""2020-12-12T12:12:12Z"",
                        ""workflowStepDueDate"": null,
                        ""description"": ""null"",
                        ""referenceNumber"": null,
                        ""jobNumber"": null
                    },
                    ""targetLocaleId"": ""fr"",
                    ""originalLocaleId"": ""en"",
                    ""workflowStep"": {
                        ""workflowStepName"": ""{workflowStepName}"",
                        ""workflowStepUid"": ""{workflowStepUid}"",
                        ""workflowStepType"": ""TRANSLATION""
                    },
                    ""wordCount"": 100,
                    ""stringCount"": 100,
                    ""issues"": {
                        ""issuesCount"": 0
                    },
                    ""claiming"": {
                        ""isClaimable"": false,
                        ""isUnclaimable"": false,
                        ""claimableWordCount"": 0
                    },
                    ""offlineWorkEnabled"": false,
                    ""gracefulWindow"": null,
                    ""isActionable"": null,
                    ""isDeclined"": null,
                    ""isContentUpdated"": null
                        }
                    ]
                }
            }
        }";

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
            var authClient = new Mock<AuthApiUser>("test", "test");
            var auth = new OAuthAuthenticationStrategy(authClient.Object);
            authClient.CallBase = true;
            authClient.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(ValidAuthRespone);
            authClient.Setup(foo => foo.PrepareJsonPostRequest(It.IsAny<string>(), It.IsAny<Object>(), It.IsAny<string>())).Returns((WebRequest)null);
            return auth;
        }

        [TestMethod]
        public void ShouldRetrieveDasboardData()
        {
            // Arrange
            var auth = GetAuth();
            var client = new Mock<TranslatorDashboardApiClient>(auth)
            {
                CallBase = true
            };
            client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(DashboardValidationResponse);

            // Act
            var data = client.Object.GetDashboardData();

            // Assert
            Assert.AreEqual(1, data.items.Count());
            Assert.AreEqual("TEST_PROJECT_NAME", data.items.First().project.projectName);
            Assert.AreEqual("TEST_PROJECT_ID", data.items.First().project.projectId);
            client.Verify(t => t.GetResponse(It.IsAny<WebRequest>()), Times.Once);
        }
    }
}
