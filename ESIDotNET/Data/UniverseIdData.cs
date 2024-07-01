using Newtonsoft.Json;

namespace ESIDotNET.Data
{
    public class UniverseIdData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }
    }
}
