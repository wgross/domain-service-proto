using System.Threading.Tasks;

namespace Domain.Client.Authorization
{
    public interface IDomainUserTokenProvider : ITokenProvider
    {
        Task FetchToken(string clientId, string clientSecret, string userId, string userSecret, string[] scopes);
    }
}