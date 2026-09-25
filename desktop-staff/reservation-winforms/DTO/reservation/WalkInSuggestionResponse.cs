using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace reservation_winforms.DTO.reservation
{
    public class WalkInSuggestionResponse
    {
        [JsonProperty("suggestionId")]
        public long SuggestionId { get; set; }

        [JsonProperty("suggestedTables")]
        public List<SuggestedTableDTO> SuggestedTables { get; set; }

        [JsonProperty("lockExpiresAt")]
        public DateTime LockExpiresAt { get; set; }

        [JsonProperty("guestCount")]
        public int GuestCount { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("availabilityType")]
        public string AvailabilityType { get; set; }

        public class SuggestedTableDTO
        {
            [JsonProperty("tableId")]
            public long TableId { get; set; }

            [JsonProperty("capacity")]
            public int Capacity { get; set; }

            [JsonProperty("availabilityType")]
            public string AvailabilityType { get; set; }
        }
    }
}