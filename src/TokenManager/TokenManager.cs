using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager
{
    public class TokenManager : ITokenManager
    {
        static readonly HttpClient client = new HttpClient();

        public async Task<string> FetchAccessToken(string authority, string clientId, string clientSecret, string audience, bool forceInvalidateCache = false)
        {
            var cache = MemoryCache.Default;
            var cacheKey = $"{authority}{clientId}{audience}";

            if (forceInvalidateCache)
                cache.Remove(cacheKey);

            var cachedToken = cache.GetCacheItem(cacheKey);
            var token = cachedToken?.Value as Token;

            if (token is null)
            {
                token = await GetTokenFromIdentityServer(authority, clientId, clientSecret, audience);

                var cachePolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(token.Expires - 10),
                };

                cache.Add(cacheKey, token, cachePolicy);
            }

            return token.AccessToken;
        }

        private static async Task<Token> GetTokenFromIdentityServer(string authority, string clientId, string clientSecret, string audience)
        {
            var authorityUri = new Uri(authority);

            var requestContent =
                $"{{" +
                    $"\"client_id\":\"{clientId}\"," +
                    $"\"client_secret\":\"{clientSecret}\"," +
                    $"\"audience\":\"{audience}\"," +
                    $"\"grant_type\":\"client_credentials\"" +
                $"}}";

            var response = await client.PostAsync(new Uri(authorityUri, "oauth/token"), new StringContent(requestContent, Encoding.UTF8, "application/json")).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var bearer = JsonConvert.DeserializeObject<Token>(responseString);
            return bearer;
        }
    }
}
