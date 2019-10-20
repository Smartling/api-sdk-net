using System;
using Newtonsoft.Json;

namespace Smartling.Api.Authentication
{
  public class AuthApiClient : ApiClientBase
  {
        private readonly string userIdentifier;
        private readonly string userSecret;

        protected virtual string AuthApiV2Authenticate { get { return "/auth-api/v2/authenticate"; } }
        protected virtual string AuthApiV2Refresh { get { return "/auth-api/v2/authenticate/refresh"; } }

        public AuthApiClient(string userIdentifier, string userSecret)
        {
          this.userIdentifier = userIdentifier;
          this.userSecret = userSecret;
        }

        public virtual AuthResponse Authenticate()
        {
          var command = new AuthCommand() { userIdentifier = this.userIdentifier, userSecret = this.userSecret };
          var request = PrepareJsonPostRequest(ApiGatewayUrl + AuthApiV2Authenticate, command, String.Empty);
          var jsonResponse = GetResponse(request);
          var authResponse = JsonConvert.DeserializeObject<AuthResponseWrapper>(jsonResponse);

          return authResponse.response;
        }

        public virtual AuthResponse Refresh(string refreshToken)
        {
          var command = new RefreshCommand() { refreshToken = refreshToken };
          var request = PrepareJsonPostRequest(ApiGatewayUrl + AuthApiV2Refresh, command, string.Empty);
          var jsonResponse = GetResponse(request);
          var refreshResponse = JsonConvert.DeserializeObject<AuthResponseWrapper>(jsonResponse);

          return refreshResponse.response;
        }
   }
}