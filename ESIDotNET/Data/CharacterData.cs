using Newtonsoft.Json;

namespace ESIDotNET.Data
{
    public class CharacterData
    {
        [JsonProperty("alliance_id")]
        public int AllianceId { get; set; }

        [JsonProperty("birthday")]
        public DateTime Birthday { get; set; }

        [JsonProperty("corporation_id")]
        public int CorporationId { get; set; }

        [JsonProperty("description")]
        public required string Bio { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("security_status")]
        public float SecurityStatus { get; set; }
    }
}
