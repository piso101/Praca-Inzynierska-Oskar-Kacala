using BackEnd.Domain.Entities;

namespace BackEnd.Domain.Repositories
{
    public interface ISurveyResponseRepository : IRepository<SurveyResponse>
    {
        Task<IReadOnlyList<SurveyResponse>> ListBySurveyAsync(long surveyId, CancellationToken cancellationToken = default);
    }
}


