using Newtonsoft.Json;

namespace reservation_winforms.DTO.table
{
    public class TableRequest
    {
        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("version")]
        public long? Version { get; set; }
    }
}