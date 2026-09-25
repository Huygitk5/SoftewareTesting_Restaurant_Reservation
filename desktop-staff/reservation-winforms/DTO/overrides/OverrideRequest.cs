using Newtonsoft.Json;

namespace reservation_winforms.DTO.overrides
{
    public class OverrideRequest
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}