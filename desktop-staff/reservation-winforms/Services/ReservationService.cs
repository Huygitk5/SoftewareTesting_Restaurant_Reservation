using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Thêm thư viện này để đọc JSON
using reservation_winforms.DTO;
using reservation_winforms.DTO.reservation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace reservation_winforms.Services
{
    public class ReservationService
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

        // 1. Gửi yêu cầu gợi ý (Soft-lock bàn) cho khách Walk-in
        public async Task<(bool IsSuccess, WalkInSuggestionResponse Data, string Message)> SuggestWalkInAsync(WalkInRequest request)
        {
            try
            {
                ApiClient.AttachToken();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/walk-in/suggest", jsonContent);
                var contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<WalkInSuggestionResponse>(contentString);
                    return (true, data, "Gợi ý thành công");
                }
                return (false, null, GetErrorMessage(contentString));
            }
            catch (Exception ex) { return (false, null, $"Lỗi kết nối: {ex.Message}"); }
        }

        // 2. Lễ tân bấm XÁC NHẬN (Confirm) cho khách Walk-in
        public async Task<(bool IsSuccess, string Message)> ConfirmWalkInAsync(long suggestionId)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/walk-in/confirm/{suggestionId}", null);
                var contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) return (true, "Xác nhận thành công");
                return (false, GetErrorMessage(contentString));
            }
            catch (Exception ex) { return (false, $"Lỗi kết nối: {ex.Message}"); }
        }

        // 3. Lễ tân bấm HỦY (Cancel) bảng gợi ý Walk-in
        public async Task<bool> CancelWalkInSuggestionAsync(long suggestionId)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/walk-in/cancel-suggestion/{suggestionId}", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // 4. Lấy danh sách các Options ghép bàn
        public async Task<(bool IsSuccess, WalkInOptionResponse Data, string Message)> GetWalkInOptionsAsync(int guestCount)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/reservations/walk-in/options?guestCount={guestCount}");
                var contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<WalkInOptionResponse>(contentString);
                    return (true, data, "Thành công");
                }
                return (false, null, GetErrorMessage(contentString));
            }
            catch (Exception ex) { return (false, null, $"Lỗi kết nối: {ex.Message}"); }
        }

        // 5. Lấy danh sách đơn đặt trước Online
        public async Task<(bool IsSuccess, List<ReservationResponse> Data, string Message)> GetUpcomingReservationsAsync(int minutes = 1440)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/reservations/upcoming?minutes={minutes}");
                var contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, JsonConvert.DeserializeObject<List<ReservationResponse>>(contentString), "Thành công");

                return (false, null, GetErrorMessage(contentString));
            }
            catch (Exception ex) { return (false, null, $"Lỗi: {ex.Message}"); }
        }

        // 6. Gọi API Check-in cho đơn Online
        public async Task<(bool IsSuccess, string Message)> CheckInAsync(long reservationId)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/{reservationId}/check-in", null);
                var contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) return (true, "Check-in thành công");
                return (false, GetErrorMessage(contentString));
            }
            catch (Exception ex) { return (false, $"Lỗi: {ex.Message}"); }
        }

        // 7. Gọi API Đổi Bàn
        public async Task<(bool IsSuccess, ReservationResponse Data, string Message)> ChangeTableAsync(long reservationId, ChangeTableRequest request)
        {
            try
            {
                ApiClient.AttachToken();
                string json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/{reservationId}/change-table", content);
                var resContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, JsonConvert.DeserializeObject<ReservationResponse>(resContent), "Thành công");

                return (false, null, GetErrorMessage(resContent));
            }
            catch (Exception ex) { return (false, null, $"Lỗi kết nối: {ex.Message}"); }
        }

        // 8. Lấy danh sách các đơn đang ở trạng thái SEATED
        public async Task<(bool IsSuccess, List<ReservationResponse> Data, string Message)> GetActiveReservationsAsync()
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.GetAsync($"{ApiClient.BaseUrl}/reservations/active");
                var contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, JsonConvert.DeserializeObject<List<ReservationResponse>>(contentString), "Thành công");

                return (false, null, GetErrorMessage(contentString));
            }
            catch (Exception ex) { return (false, null, $"Lỗi: {ex.Message}"); }
        }

        // 9. Gọi API Check-out
        public async Task<(bool IsSuccess, string Message)> CheckOutAsync(long reservationId)
        {
            try
            {
                ApiClient.AttachToken();
                var response = await ApiClient.Client.PostAsync($"{ApiClient.BaseUrl}/reservations/{reservationId}/check-out", null);
                var contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) return (true, "Thanh toán thành công");
                return (false, GetErrorMessage(contentString));
            }
            catch (Exception ex) { return (false, $"Lỗi kết nối: {ex.Message}"); }
        }
    }
}