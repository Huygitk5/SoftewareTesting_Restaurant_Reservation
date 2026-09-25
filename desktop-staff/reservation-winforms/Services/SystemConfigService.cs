using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reservation_winforms.DTO.config;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class SystemConfigService
    {
        // Copy y nguyên hàm xử lý lỗi từ các service khác của bạn
        private string GetErrorMessage(string jsonContent)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonContent)) return "Lỗi không xác định từ máy chủ.";

                var jObject = JObject.Parse(jsonContent);

                if (jObject["message"] != null) return jObject["message"].ToString();

                if (jObject["error"] != null) return jObject["error"].ToString();
            }
            catch
            {
                if (jsonContent.Length > 100) return jsonContent.Substring(0, 100) + "...";
            }

            return jsonContent;
        }

        // 1. Lấy danh sách cấu hình
        public async Task<(bool IsSuccess, List<SystemConfigDTO> Data, string Message)> GetAllConfigsAsync()
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/configs");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var configs = JsonConvert.DeserializeObject<List<SystemConfigDTO>>(content);
                    return (true, configs, "Thành công");
                }

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, $"Lỗi kết nối: {ex.Message}"); }
        }

        // 2. Cập nhật cấu hình
        public async Task<(bool IsSuccess, SystemConfigDTO Data, string Message)> UpdateConfigAsync(string key, string newValue)
        {
            try
            {
                var requestData = new SystemConfigDTO
                {
                    ConfigKey = key,
                    ConfigValue = newValue
                };

                ApiClient.AttachToken();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await ApiClient.Client.PutAsync($"{ApiClient.BaseUrl}/configs/{key}", jsonContent);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var updatedConfig = JsonConvert.DeserializeObject<SystemConfigDTO>(content);
                    return (true, updatedConfig, "Cập nhật thành công!");
                }

                return (false, null, GetErrorMessage(content));
            }
            catch (Exception ex) { return (false, null, $"Lỗi kết nối: {ex.Message}"); }
        }
    }
}