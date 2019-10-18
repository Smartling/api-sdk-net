using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Model;
using System.Security.Authentication;

namespace Smartling.Api.Dashboard
{
    public class DashboardApiClient : ApiClientBase
    {
        private readonly IAuthenticationStrategy _auth;
        private const string DashboardUrl = "/translator-dashboard-api/v2/jobs";

        public DashboardApiClient(IAuthenticationStrategy auth)
        {
            _auth = auth;
        }

        public virtual DashboardData GetDashboardData()
        {
            var uriBuilder = GetRequestStringBuilder(DashboardUrl);
            var request = PrepareGetRequest(uriBuilder.ToString(), _auth.GetToken());
            JObject response;

            try
            {
                response = JObject.Parse(GetResponse(request));
            }
            catch (AuthenticationException)
            {
                request = PrepareGetRequest(uriBuilder.ToString(), _auth.GetToken(true));
                response = JObject.Parse(GetResponse(request));
            }
            return JsonConvert.DeserializeObject<DashboardData>(response["response"]["data"].ToString());
        }
    }
}
