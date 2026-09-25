using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace reservation_winforms.DTO.reservation
{
    public class WalkInRequest
    {
        [JsonProperty("guestCount")]
        public int GuestCount { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("startTime")]
        public DateTime? StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime? EndTime { get; set; }

        [JsonProperty("tableId")]
        public List<long> TableId { get; set; }

        [JsonProperty("mergeTables")]
        public bool MergeTables { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
}