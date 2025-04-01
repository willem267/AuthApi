using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<User?> getUserByUsername(string username)
        {
            User user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                return null;
            }
            return user;
        }
    }
}
