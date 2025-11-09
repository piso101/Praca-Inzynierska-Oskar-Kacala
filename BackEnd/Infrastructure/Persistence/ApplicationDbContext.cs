using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
        public DbSet<Survey> Surveys => Set<Survey>();
        public DbSet<SurveyQuestion> SurveyQuestions => Set<SurveyQuestion>();
        public DbSet<QuestionElement> QuestionElements => Set<QuestionElement>();
        public DbSet<SurveyResponse> SurveyResponses => Set<SurveyResponse>();
        public DbSet<ResponseAnswer> ResponseAnswers => Set<ResponseAnswer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserAccountConfiguration());
            modelBuilder.ApplyConfiguration(new SurveyConfiguration());
            modelBuilder.ApplyConfiguration(new SurveyQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionElementConfiguration());
            modelBuilder.ApplyConfiguration(new SurveyResponseConfiguration());
            modelBuilder.ApplyConfiguration(new ResponseAnswerConfiguration());
        }
    }
}