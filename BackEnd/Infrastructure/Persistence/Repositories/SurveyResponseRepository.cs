using BackEnd.Domain.Entities;
using BackEnd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence.Repositories
{
    public class SurveyResponseRepository : EfRepository<SurveyResponse>, ISurveyResponseRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SurveyResponseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<SurveyResponse>> ListBySurveyAsync(long surveyId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SurveyResponses
                .Include(r => r.Answers)
                .Where(r => r.SurveyId == surveyId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}