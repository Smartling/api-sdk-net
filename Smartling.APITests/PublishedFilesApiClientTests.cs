using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smartling.Api.Authentication;
using Smartling.Api.Model;
using Smartling.Api.Project;
using Smartling.Api.PublishedFiles;

namespace Smartling.ApiTests
{
  [TestClass]
  public class PublishedFilesApiClientTests
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

    private const string PublishedFilesApiResponse = @"{
       ""response"": {
           ""code"": ""SUCCESS"",
           ""data"": {
             ""totalCount"": ""1"",
             ""items"" : [{ ""fileUri"": ""/sitecore/content/Home/Sample"", ""localeId"": ""ru-RU"", ""publishDate"": ""2012-04-23T18:25:43.511Z"" }]        
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
    public void PublishedFiles_Should_Parse_Response()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<PublishedFilesApiClient>(auth, "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(PublishedFilesApiResponse);

      var search = new RecentlyPublishedSearch(DateTime.Now.AddDays(-5));
      search.FileUris.Add("/content/Home/6A2CD795_en.xml");
      search.LocaleIds.Add("ru-RU");
      search.Limit = 10;
      search.Offset = 0;

      // Act
      var result = client.Object.GetRecentlyPublished(search);

      // Assert
      Assert.AreEqual(1, result.items.Count);
    }


    [TestMethod]
    public void PublishedFiles_Should_Encode_Parameters()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<PublishedFilesApiClient>(auth, "test");
      client.CallBase = true;
      WebRequest saveObject = null;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Callback<WebRequest>((obj) => saveObject = obj).Returns(PublishedFilesApiResponse);

      var search = new RecentlyPublishedSearch(DateTime.Now.AddDays(-5));
      search.FileUris.Add("/content/Home/John & Doe_en.xml");

      // Act
      var result = client.Object.GetRecentlyPublished(search);

      // Assert
      Assert.IsTrue(saveObject.RequestUri.ToString().EndsWith("&fileUris[]=%2Fcontent%2FHome%2FJohn+%26+Doe_en.xml"));
    }

    [TestMethod]
    public void PublishedFiles_Should_Validate_Limit()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<PublishedFilesApiClient>(auth, "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(PublishedFilesApiResponse);

      var search = new RecentlyPublishedSearch(DateTime.Now.AddDays(-5));
      search.FileUris.Add("/content/Home/6A2CD795_en.xml");
      search.LocaleIds.Add("ru-RU");
      search.Limit = -10;
      search.Offset = 0;

      // Act
      var correctExceptionThrown = false;

      try
      {
        var result = client.Object.GetRecentlyPublished(search);
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("Limit must not be negative"))
          correctExceptionThrown = true;
      }
      
      // Assert
      Assert.IsTrue(correctExceptionThrown);
    }

    [TestMethod]
    public void PublishedFiles_Should_Validate_Offset()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<PublishedFilesApiClient>(auth, "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(PublishedFilesApiResponse);

      var search = new RecentlyPublishedSearch(DateTime.Now.AddDays(-5));
      search.FileUris.Add("/content/Home/6A2CD795_en.xml");
      search.LocaleIds.Add("ru-RU");
      search.Limit = 0;
      search.Offset = 10;

      // Act
      var correctExceptionThrown = false;

      try
      {
        var result = client.Object.GetRecentlyPublished(search);
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("Offset requires Limit > 0"))
          correctExceptionThrown = true;
      }

      // Assert
      Assert.IsTrue(correctExceptionThrown);
    }

    [TestMethod]
    public void PublishedFiles_Should_Validate_Date()
    {
      // Arrange
      var auth = GetAuth();
      var client = new Mock<PublishedFilesApiClient>(auth, "test");
      client.CallBase = true;
      client.Setup(foo => foo.GetResponse(It.IsAny<WebRequest>())).Returns(PublishedFilesApiResponse);

      var search = new RecentlyPublishedSearch(DateTime.Now.AddDays(-15));

      // Act
      var correctExceptionThrown = false;

      try
      {
        var result = client.Object.GetRecentlyPublished(search);
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("PublishedAfter is limited to 14 days back from now"))
          correctExceptionThrown = true;
      }

      // Assert
      Assert.IsTrue(correctExceptionThrown);
    }
  }
}