using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace reservation_winforms.DTO.reservation
{
    public class ReservationResponse
    {
        [JsonProperty("reservationId")]
        public long ReservationId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("guestCount")]
        public int GuestCount { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("depositAmount")]
        public double? DepositAmount { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("customerId")]
        public long? CustomerId { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("tableIds")]
        public List<long> TableIds { get; set; }
    }
}