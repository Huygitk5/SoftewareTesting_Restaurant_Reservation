using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace reservation_winforms.Services
{
    public static class ApiClient
    {
        public static readonly HttpClient Client = new HttpClient();
        public static readonly string BaseUrl = ConfigurationManager.AppSettings["base.uri"];

        public static void AttachToken()
        {
            if (!string.IsNullOrEmpty(GlobalState.StaffToken))
            {
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", GlobalState.StaffToken);

                Client.DefaultRequestHeaders.Remove("X-Staff-Token");
                Client.DefaultRequestHeaders.Add("X-Staff-Token", GlobalState.StaffToken);
            }
        }
    }
}