using System;
using Newtonsoft.Json;

namespace reservation_winforms.DTO.overrides
{
    public class OverrideLogResponse
    {
        [JsonProperty("logId")]
        public long LogId { get; set; }

        [JsonProperty("reservationId")]
        public long ReservationId { get; set; }

        [JsonProperty("accountId")]
        public long AccountId { get; set; }

        [JsonProperty("accountUsername")]
        public string AccountUsername { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}