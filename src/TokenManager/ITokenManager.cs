using System.Threading.Tasks;

namespace TokenManager
{
    public interface ITokenManager
    {
        Task<string> FetchAccessToken(string authority, string clientId, string clientSecret, string audience, bool forceInvalidateCache = false);
    }
}