using Newtonsoft.Json;

namespace Smartling.Api.Authentication
{
    public class AuthApiUser : AuthApiClient
    {
        private const string AUTH_API_V2_AUTHENTICATE_USER = "/auth-api/v2/authenticate/user";
        private const string AUTH_API_V2_REFRESH_USER = "/auth-api/v2/authenticate/refresh/user";
        private readonly string _userIdentifier;
        private readonly string _userSecret;

        public AuthApiUser(string userIdentifier, string userSecret) : base(userIdentifier, userSecret)
        {
            _userIdentifier = userIdentifier;
            _userSecret = userSecret;
        }

        public override AuthResponse Authenticate()
        {
            var command = new AuthCommand() { userIdentifier = _userIdentifier, userSecret = _userSecret };
            var request = PrepareJsonPostRequest(ApiGatewayUrl + AUTH_API_V2_AUTHENTICATE_USER, command, string.Empty);
            var jsonResponse = GetResponse(request);
            var authResponse = JsonConvert.DeserializeObject<AuthResponseWrapper>(jsonResponse);

            return authResponse.response;
        }

        public override AuthResponse Refresh(string refreshToken)
        {
            var command = new RefreshCommand() { refreshToken = refreshToken };
            var request = PrepareJsonPostRequest(ApiGatewayUrl + AUTH_API_V2_REFRESH_USER, command, string.Empty);
            var jsonResponse = GetResponse(request);
            var refreshResponse = JsonConvert.DeserializeObject<AuthResponseWrapper>(jsonResponse);

            return refreshResponse.response;
        }
    }
}
