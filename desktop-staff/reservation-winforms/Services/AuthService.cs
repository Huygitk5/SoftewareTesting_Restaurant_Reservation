using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Thêm thư viện này
using reservation_winforms.DTO.auth;
using reservation_winforms.DTO;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class AuthService
    {
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

        public async Task<(bool IsSuccess, string Message)> LoginAsync(string username, string password)
        {
            try
            {
                var requestData = new StaffLoginRequest { Username = username, Password = password };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/staff/auth/login", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginData = JsonConvert.DeserializeObject<StaffLoginResponse>(responseString);

                    GlobalState.StaffToken = loginData.SessionToken;
                    GlobalState.CurrentUsername = loginData.Username;
                    GlobalState.Role = loginData.Role;

                    return (true, "Đăng nhập thành công!");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return (false, "Tài khoản hoặc mật khẩu không chính xác.");
                }
                else
                {
                    return (false, GetErrorMessage(responseString));
                }
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kết nối tới Server: {ex.Message}");
            }
        }

        // HÀM ĐĂNG KÝ TÀI KHOẢN MỚI (Dành cho Quản lý)
        public async Task<(bool IsSuccess, string Message)> RegisterStaffAsync(string username, string password, string role = "RECEPTIONIST")
        {
            try
            {
                var requestData = new StaffRegisterRequest
                {
                    Username = username,
                    Password = password,
                    Role = role
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                ApiClient.AttachToken();

                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/staff/auth/register", jsonContent);
                var resultString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Tạo tài khoản thành công!");
                }
                else
                {
                    return (false, GetErrorMessage(resultString));
                }
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kết nối: {ex.Message}");
            }
        }
    }
}