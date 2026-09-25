using Newtonsoft.Json;
using System;

namespace reservation_winforms.DTO.table
{
    public class FloorMapTableResponse
    {
        [JsonProperty("tableId")]
        public long TableId { get; set; }

        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("active")]
        public bool IsActive { get; set; }

        [JsonProperty("currentReservationId")]
        public long? CurrentReservationId { get; set; }

        [JsonProperty("currentCustomerName")]
        public string CurrentCustomerName { get; set; }

        [JsonProperty("currentReservationStatus")]
        public string CurrentReservationStatus { get; set; }

        [JsonProperty("currentReservationTime")]
        public DateTime? CurrentReservationTime { get; set; }

        [JsonProperty("nextReservationTime")]
        public DateTime? NextReservationTime { get; set; }

    }
}