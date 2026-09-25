using Newtonsoft.Json;

namespace reservation_winforms.DTO.waitList
{
    public class WaitlistRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("guestCount")]
        public int GuestCount { get; set; }

        [JsonProperty("allowShortSeating")]
        public bool AllowShortSeating { get; set; }
    }
}
