using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reservation_winforms.DTO.overrides;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class OverrideService
    {
        private string GetErrorMessage(string jsonContent)
        {
            try
            {
                var jObject = JObject.Parse(jsonContent);
                if (jObject["message"] != null)
                {
                    return jObject["message"].ToString();
                }
            }
            catch { }
            return jsonContent;
        }

        // 1. GỌI API CƯỠNG CHẾ TRẢ BÀN (POST)
        public async Task<(bool IsSuccess, string Message)> OverrideReservationAsync(long reservationId, string reason)
        {
            try
            {
                ApiClient.AttachToken();

                var requestData = new { reason = reason };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");

                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/{reservationId}/override", jsonContent);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cưỡng chế thanh toán và ghi log thành công!");
                }

                return (false, GetErrorMessage(content));
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kết nối: {ex.Message}");
            }
        }

        public async Task<(bool IsSuccess, List<OverrideLogResponse> Data, string Message)> GetLogsAsync(DateTime from, DateTime to)
        {
            try
            {
                ApiClient.AttachToken();

                string fromStr = from.ToString("yyyy-MM-ddTHH:mm:ss");
                string toStr = to.ToString("yyyy-MM-ddTHH:mm:ss");

                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/override-logs?from={fromStr}&to={toStr}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<List<OverrideLogResponse>>(content);
                    return (true, data, "OK");
                }

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex)
            {
                return (false, null, $"Lỗi kết nối: {ex.Message}");
            }
        }
    }
}