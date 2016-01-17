using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Smartling.Api.Model;

namespace Smartling.Api.Authentication
{
  public class ApiClientBase
  {
    protected const string DEFAULT_API_GATEWAY_URL = "https://api.smartling.com";
    private const string PostMethod = "POST";
    private const string JsonContentType = "application/json; charset=UTF-8";
    private const string JsonAccept = "application/json";

    public virtual string GetResponse(WebRequest request)
    {
      try
      {
        using (var response = request.GetResponse())
        using (var stream = response.GetResponseStream())
        using (var reader = new StreamReader(stream))
        {
          return reader.ReadToEnd();
        }
      }
      catch (WebException e)
      {
        if (e.Response == null)
        {
          throw;
        }

        using (WebResponse response = e.Response)
        {
          using (Stream data = response.GetResponseStream())
          using (var reader = new StreamReader(data))
          {
            string text = reader.ReadToEnd();
            var error = JsonConvert.DeserializeObject<Error>(text);
            string messages = error.response.errors.Aggregate(
              string.Empty,
              (current, message) => string.Format("{0}{1}{2} ({3})", current, Environment.NewLine, message.message, message.details));

            throw new Exception(error.response.code + ": " + messages, e);
          }
        }
      }
    }

    public virtual WebRequest CreatePostRequest(string url, object command)
    {
      var request = (HttpWebRequest)WebRequest.Create(url);
      var json = JsonConvert.SerializeObject(command);
      byte[] postBytes = Encoding.UTF8.GetBytes(json);

      request.Method = PostMethod;
      request.ContentType = JsonContentType;
      request.Accept = JsonAccept;
      request.ContentLength = postBytes.Length;

      Stream requestStream = request.GetRequestStream();
      requestStream.Write(postBytes, 0, postBytes.Length);
      requestStream.Close();
      return request;
    }
  }
}