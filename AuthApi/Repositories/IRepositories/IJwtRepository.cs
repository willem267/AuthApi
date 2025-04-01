namespace AuthApi.Repositories.IRepositories
{
    public interface IJwtRepository
    {
        Task<string> generateJwtKey(string username);

    }
}
