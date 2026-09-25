using Newtonsoft.Json;

namespace reservation_winforms.DTO.config
{
    public class SystemConfigDTO
    {
        [JsonProperty("configKey")]
        public string ConfigKey { get; set; }

        [JsonProperty("configValue")]
        public string ConfigValue { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}