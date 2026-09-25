using Newtonsoft.Json;

namespace reservation_winforms.DTO.report
{
    public class DailyReservationReport
    {
        [JsonProperty("date")]
        public string Date { get; set; } // Chứa "yyyy-MM-dd", "yyyy-MM", hoặc "yyyy"

        [JsonProperty("totalOnline")]
        public long TotalOnline { get; set; }

        [JsonProperty("totalWalkIn")]
        public long TotalWalkIn { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("noShowCount")]
        public long NoShowCount { get; set; }
    }
}