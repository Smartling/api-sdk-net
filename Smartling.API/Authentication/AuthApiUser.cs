namespace Smartling.Api.Authentication
{
    public class AuthApiUser : AuthApiClient
    {
        protected override string AuthApiV2Authenticate { get { return "/auth-api/v2/authenticate/user"; } }

        public AuthApiUser(string userIdentifier, string userSecret) : base(userIdentifier, userSecret)
        {
        }
    }
}
