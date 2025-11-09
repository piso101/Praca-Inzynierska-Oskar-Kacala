using BackEnd.Domain.Entities;

namespace BackEnd.Domain.Repositories
{
    public interface ISurveyRepository : IRepository<Survey>
    {
        Task<Survey?> GetWithQuestionsAsync(long id, CancellationToken cancellationToken = default);
    }
}