using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace reservation_winforms.DTO.waitList
{
    public class WaitlistResponse
    {
        [JsonProperty("waitlistId")]
        public long WaitlistId { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("guestCount")]
        public int GuestCount { get; set; }

        [JsonProperty("allowShortSeating")]
        public bool AllowShortSeating { get; set; }

        [JsonProperty("joinedAt")]
        public DateTime JoinedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("readyToSeat")]
        public bool ReadyToSeat { get; set; }

        [JsonProperty("seatingType")]
        public string SeatingType { get; set; }

        [JsonProperty("suggestedAvailableUntil")]
        public DateTime? SuggestedAvailableUntil { get; set; }

        [JsonProperty("suggestedTableIds")]
        public List<long> SuggestedTableIds { get; set; }
    }
}