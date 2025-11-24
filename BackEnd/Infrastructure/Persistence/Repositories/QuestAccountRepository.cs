using BackEnd.Domain.Entities;
using BackEnd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence.Repositories
{
    public class QuestAccountRepository : EfRepository<QuestAccount>, IQuestAccountRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public QuestAccountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<QuestAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _dbContext.QuestAccounts.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public Task<QuestAccount?> GetByVerificationTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return _dbContext.QuestAccounts.FirstOrDefaultAsync(
                u => u.EmailVerificationToken == token && u.TokenExpiresAt > DateTime.UtcNow, 
                cancellationToken);
        }
    }
}

