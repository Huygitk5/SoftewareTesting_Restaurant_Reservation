using System.Collections.Generic;
using Newtonsoft.Json;

namespace reservation_winforms.DTO.reservation
{
    public class ChangeTableRequest
    {
        [JsonProperty("tableIds")]
        public List<long> TableIds { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
