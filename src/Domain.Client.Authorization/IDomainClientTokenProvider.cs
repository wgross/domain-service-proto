using System.Threading.Tasks;

namespace Domain.Client.Authorization
{
    /// <summary>
    /// Provide an client authentication token.
    /// </summary>
    public interface IDomainClientTokenProvider
    {
        Task FetchToken(string clientId, string clientSecret, string[] scopes);

        string GetToken();
    }
}