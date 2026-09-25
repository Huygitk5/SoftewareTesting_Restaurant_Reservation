using Newtonsoft.Json;

namespace reservation_winforms.DTO.auth
{
    public class StaffLoginResponse
    {
        [JsonProperty("sessionToken")]
        public string SessionToken { get; set; }

        [JsonProperty("accountId")]
        public long AccountId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }
}