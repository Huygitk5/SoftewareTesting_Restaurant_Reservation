using Newtonsoft.Json;
using System;

namespace reservation_winforms.DTO.table
{
    public class TableUpdate
    {
        [JsonProperty("tableId")]
        public long TableId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}