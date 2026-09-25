using Newtonsoft.Json;
using System;

namespace reservation_winforms.DTO.table
{
    public class TableAlertMessage
    {
        [JsonProperty("tableId")]
        public long TableId { get; set; }

        [JsonProperty("alertType")]
        public string AlertType { get; set; } // "START_BLINK" hoặc "STOP_BLINK"
    }
}