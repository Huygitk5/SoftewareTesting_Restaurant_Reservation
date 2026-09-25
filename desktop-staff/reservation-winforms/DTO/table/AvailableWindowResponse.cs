using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace reservation_winforms.DTO.table
{
    public class AvailableWindowResponse
    {
        [JsonProperty("tableId")]
        public long? TableId { get; set; }

        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("availability")]
        public string Availability { get; set; } // "FULL_AVAILABLE" hoặc "PARTIAL_AVAILABLE"

        [JsonProperty("availableUntil")]
        public DateTime? AvailableUntil { get; set; }

        [JsonProperty("mergeCandidateIds")]
        public List<long> MergeCandidateIds { get; set; }
    }
}
