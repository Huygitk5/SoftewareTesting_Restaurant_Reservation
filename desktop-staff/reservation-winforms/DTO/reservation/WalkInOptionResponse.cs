using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace reservation_winforms.DTO.reservation
{
    public class WalkInOptionResponse
    {
        [JsonProperty("groups")]
        public List<OptionGroup> Groups { get; set; }

        public class OptionGroup
        {
            [JsonProperty("groupName")]
            public string GroupName { get; set; }

            [JsonProperty("options")]
            public List<TableOption> Options { get; set; }
        }

        public class TableOption
        {
            [JsonProperty("tableIds")]
            public List<long> TableIds { get; set; }

            [JsonProperty("totalCapacity")]
            public int TotalCapacity { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("availableUntil")]
            public DateTime? AvailableUntil { get; set; }
        }
    }
}