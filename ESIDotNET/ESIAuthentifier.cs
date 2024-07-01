using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ESIDotNET
{
    public static class ESIAuthentifier
    {
        public const string AuthBaseURL = "https://login.eveonline.com/v2/oauth/authorize";
        public const string ClientID = "984a86320cb64475bccee6cd35ab8638";
        public const string CallbackUri = "https://localhost/callback/";
        public static readonly string[] Scopes = ["esi-skills.read_skills.v1", "esi-skills.read_skillqueue.v1"];

        private static string _code_challenge = null!;
        private static string _code_verifier = null!;

        private static readonly List<string> _unresolvedStates = [];

        private static string GenerateAuthentificationUrl()
        {
            (_code_challenge, _code_verifier) = GeneratePKCE();

            var state = GenerateState();

            var callbackUri = HttpUtility.UrlEncode(CallbackUri);

            var scopes = string.Join("%20", Scopes);

            var url = new StringBuilder(AuthBaseURL);
            url.Append("?response_type=code");
            url.Append("&redirect_uri=");
            url.Append(callbackUri);
            url.Append("&client_id=");
            url.Append(ClientID);
            url.Append("&state=");
            url.Append(state);
            url.Append("&scope=");
            url.Append(scopes);
            url.Append("&code_challenge_method=S256");
            url.Append("&code_challenge=");
            url.Append(_code_challenge);

            return url.ToString();
        }

        private static string GenerateState()
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            var stateSource = DateTime.Now.ToString() + Guid.NewGuid().ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(stateSource);

            var hash = SHA512.HashData(bytes);

            char[] alphanumHash = new char[64];
            for (int i = 0; i < alphanumHash.Length; i++)
            {
                alphanumHash[i] = chars[hash[i] % chars.Length];
            }

            var state = new string(alphanumHash);

            _unresolvedStates.Add(state);

            return state;
        }

        public static void GetAuthentificationCode()
        {
            var url = GenerateAuthentificationUrl();
            System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// Generates a code_verifier and the corresponding code_challenge
        /// </summary>
        private static (string code_challenge, string verifier) GeneratePKCE(int size = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[size];
            rng.GetBytes(randomBytes);
            var verifier = Base64UrlEncode(randomBytes);

            var buffer = Encoding.UTF8.GetBytes(verifier);
            var hash = SHA256.HashData(buffer);
            var challenge = Base64UrlEncode(hash);

            return (challenge, verifier);
        }

        private static string Base64UrlEncode(byte[] data) =>
            Convert.ToBase64String(data)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
    }
}
