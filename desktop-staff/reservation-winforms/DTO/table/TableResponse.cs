using Newtonsoft.Json;

namespace reservation_winforms.DTO.table
{
    public class TableResponse
    {
        [JsonProperty("tableId")]
        public long TableId { get; set; }

        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("active")]
        public bool IsActive { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}