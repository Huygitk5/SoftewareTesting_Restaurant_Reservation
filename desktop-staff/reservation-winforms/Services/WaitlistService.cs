using Newtonsoft.Json;
using reservation_winforms.DTO.waitList;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class WaitlistService
    {
        private string GetErrorMessage(string content)
        {
            try
            {
                var j = Newtonsoft.Json.Linq.JObject.Parse(content);
                if (j["message"] != null) return j["message"].ToString();
            }
            catch { }
            return content;
        }

        public async Task<(bool IsSuccess, List<WaitlistResponse> Data, string Message)> GetWaitlistAsync()
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/waitlist");
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return (true, JsonConvert.DeserializeObject<List<WaitlistResponse>>(content), "OK");
                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, ex.Message); }
        }

        public async Task<(bool IsSuccess, string Message)> AddToWaitlistAsync(WaitlistRequest req)
        {
            try
            {
                ApiClient.AttachToken();
                var json = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/waitlist", json);
                if (response.IsSuccessStatusCode) return (true, "Thêm vào danh sách chờ thành công");
                return (false, GetErrorMessage(await response.Content.ReadAsStringAsync()));
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public async Task<(bool IsSuccess, string Message)> MarkActionAsync(long id, string action)
        {
            try
            {
                ApiClient.AttachToken();
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{ApiClient.BaseUrl}/waitlist/{id}/{action}");
                var response = await ApiClient.Client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thành công");
                }

                return (false, GetErrorMessage(await response.Content.ReadAsStringAsync()));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}