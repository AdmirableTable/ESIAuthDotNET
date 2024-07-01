using ESIDotNET.Data;
using Newtonsoft.Json;
using RestSharp;

namespace ESIDotNET
{
    public static class ESIService
    {
        public static ESIConfiguration? Configuration { get; private set; }

        public static void Configure(ESIConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string BaseUrl { get; } = "https://esi.evetech.net/latest/";

        private static string CharactersPath { get; } = "characters/";

        private static string UniverseIdsPath { get; } = "universe/ids/";

        #region Exceptions
        public static class Exceptions
        {
            public static Exception HttpError { get; } = new Exception("Unhandled HTTP error.");
            public static Exception NoIdForName { get; } = new Exception("No id found for requested name.");
        }
        #endregion Exceptions

        public static async Task<CharacterData> GetCharacter(int characterID)
        {
            // curl -X GET "https://esi.evetech.net/latest/characters/94134438/"

            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{CharactersPath}{characterID}/", Method.Get);

            var response = await client.ExecuteAsync(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw Exceptions.HttpError;
            }

            if (response.Content is null)
            {
                throw Exceptions.NoIdForName;
            }

            return JsonConvert.DeserializeObject<CharacterData>(response.Content) ?? throw new Exception("Character data parse error.");
        }

        public static async Task<CharacterData> GetCharacter(string characterName)
        {
            var characterId = await GetEntityId(characterName);

            return await GetCharacter(characterId);
        }

        private static async Task<int> GetEntityId(string entityName)
        {
            var idList = new List<int>();

            var resultCollection = await SearchEntitiesByName(entityName);

            if (resultCollection.Characters is null)
            {
                throw Exceptions.NoIdForName;
            }

            idList.AddRange(resultCollection.Characters.Select(c => c.Id));

            if (idList.Count == 0)
            {
                throw Exceptions.NoIdForName;
            }

            if (idList.Count > 1)
            {
                throw new Exception("More than one id matches that name.");
            }

            return idList[0];
        }

        public static async Task<UniverseIdCollectionData> SearchEntitiesByName(string name)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(UniverseIdsPath, Method.Post) { };

            var content = $"[ \"{name}\" ]";

            request.AddBody(content);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw Exceptions.HttpError;
            }

            if (response.Content is null)
            {
                throw Exceptions.NoIdForName;
            }

            return JsonConvert.DeserializeObject<UniverseIdCollectionData>(response.Content) ?? throw new Exception("Id data parse error.");
        }

        public static CorporationData GetCorporation(int corporationID)
        {
            throw new NotImplementedException();
        }

        public static CorporationData GetCorporation(string corporationName)
        {
            throw new NotImplementedException();
        }

        public static AllianceData GetAlliance(int allianceID)
        {
            throw new NotImplementedException();
        }

        public static AllianceData GetAlliance(string allianceName)
        {
            throw new NotImplementedException();
        }
    }
}
