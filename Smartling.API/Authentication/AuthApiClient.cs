using Newtonsoft.Json;

namespace Smartling.Api.Authentication
{
  public class AuthApiClient : ApiClientBase
  {
    public const string AUTH_API_V2_AUTHENTICATE = "/auth-api/v2/authenticate";
    public const string AUTH_API_V2_REFRESH = "/auth-api/v2/authenticate/refresh";
    private readonly string userIdentifier;
    private readonly string userSecret;

    public AuthApiClient(string userIdentifier, string userSecret)
    {
      this.userIdentifier = userIdentifier;
      this.userSecret = userSecret;
    }

    public virtual AuthResponse Authenticate()
    {
      var command = new AuthCommand() { userIdentifier = this.userIdentifier, userSecret = this.userSecret };
      var request = PrepareJsonPostRequest(ApiGatewayUrl + AUTH_API_V2_AUTHENTICATE, command);
      var jsonResponse = GetResponse(request);
      var authResponse = JsonConvert.DeserializeObject<AuthResponseWrapper>(jsonResponse);

      return authResponse.response;
    }

    public virtual AuthResponse Refresh(string refreshToken)
    {
      var command = new RefreshCommand() { refreshToken = refreshToken };
      var request = PrepareJsonPostRequest(ApiGatewayUrl + AUTH_API_V2_REFRESH, command);
      var jsonResponse = GetResponse(request);
      var refreshResponse = JsonConvert.DeserializeObject<AuthResponseWrapper>(jsonResponse);

      return refreshResponse.response;
    }
  }
}