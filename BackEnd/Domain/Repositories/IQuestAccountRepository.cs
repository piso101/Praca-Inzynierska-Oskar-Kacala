using BackEnd.Domain.Entities;

namespace BackEnd.Domain.Repositories
{
    public interface IQuestAccountRepository : IRepository<QuestAccount>
    {
        Task<QuestAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<QuestAccount?> GetByVerificationTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}


