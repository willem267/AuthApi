using AuthApi.Models;

namespace AuthApi.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> getUserByUsername(string username);
    }
}
