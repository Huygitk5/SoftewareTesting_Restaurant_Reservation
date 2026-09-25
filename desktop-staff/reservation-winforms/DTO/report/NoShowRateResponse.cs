using Newtonsoft.Json;

namespace reservation_winforms.DTO.report
{
    public class NoShowRateResponse
    {
        [JsonProperty("totalOnline")]
        public long TotalOnline { get; set; }

        [JsonProperty("totalWalkIn")]
        public long TotalWalkIn { get; set; }

        [JsonProperty("totalAll")]
        public long TotalAll { get; set; }

        [JsonProperty("noShowCount")]
        public long NoShowCount { get; set; }

        [JsonProperty("noShowRate")]
        public double NoShowRate { get; set; }
    }
}