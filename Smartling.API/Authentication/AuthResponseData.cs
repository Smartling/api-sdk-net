using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartling.Api.Authentication
{
  public class AuthResponseData
  {
    public string accessToken;
    public int expiresIn;
    public int refreshExpiresIn;
    public string refreshToken;
    public string tokenType;
    public string sessionState;
  }
}