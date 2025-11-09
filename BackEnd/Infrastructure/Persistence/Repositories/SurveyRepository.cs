using BackEnd.Domain.Entities;
using BackEnd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence.Repositories
{
    public class SurveyRepository : EfRepository<Survey>, ISurveyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SurveyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Survey?> GetWithQuestionsAsync(long id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Elements)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        public async Task<Survey?> GetByIdAsync(long id, Func<IQueryable<Survey>, IQueryable<Survey>>? include = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Survey> query = (IQueryable<Survey>)_dbContext;

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<long>(e, "Id") == id, cancellationToken);
        }
    }
}