namespace Domain.Client.Authorization
{
    public interface ITokenProvider
    {
        string GetAccessToken();
    }
}