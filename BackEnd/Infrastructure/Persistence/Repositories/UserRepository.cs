using BackEnd.Domain.Entities;
using BackEnd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence.Repositories
{
    public class UserRepository : EfRepository<UserAccount>, IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}



