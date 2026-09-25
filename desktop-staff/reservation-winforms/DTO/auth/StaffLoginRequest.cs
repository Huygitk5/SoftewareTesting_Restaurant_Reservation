using Newtonsoft.Json;

namespace reservation_winforms.DTO.auth
{
    public class StaffLoginRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}