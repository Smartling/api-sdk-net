using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Model.TranslationDashboard;
using System.Net;

namespace Smartling.Api.TranslatorDashboardApiClient
{
    public class TranslatorDashboard : ApiClientBase
    {
        private readonly IAuthenticationStrategy _auth;
        private const string DashboardUrl = "/translator-dashboard-api/v2/jobs";

        public TranslatorDashboard(IAuthenticationStrategy auth)
        {
            _auth = auth;
        }

        public virtual DashboardData GetDashboardData()
        {
            var uriBuilder = GetRequestStringBuilder(DashboardUrl);
            WebRequest request = null;
            JObject response;
            try
            {
                request = PrepareGetRequest(uriBuilder.ToString(), _auth.GetToken());                            
                response = JObject.Parse(GetResponse(request));
            }
            catch (Exceptions.AuthenticationException)
            {
                throw;
            }
            return JsonConvert.DeserializeObject<DashboardData>(response["response"]["data"].ToString());
        }
    }
}
