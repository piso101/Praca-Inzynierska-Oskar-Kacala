using BackEnd.Domain.Entities;

namespace BackEnd.Domain.Repositories
{
    public interface IUserRepository : IRepository<UserAccount>
    {
        Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}



