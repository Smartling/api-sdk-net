namespace Smartling.Api.Authentication
{
  public interface IAuthenticationStrategy
  {
    string GetToken();
    string GetToken(bool forceAuthentication);
  }
}