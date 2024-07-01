using Newtonsoft.Json;

namespace ESIDotNET.Data
{
    public class UniverseIdCollectionData
    {
        [JsonProperty("characters")]
        public required List<UniverseIdData> Characters { get; set; }
    }
}
