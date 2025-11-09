using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Infrastructure.Persistence.EntityConfigurations
{
    public class SurveyResponseConfiguration : IEntityTypeConfiguration<SurveyResponse>
    {
        public void Configure(EntityTypeBuilder<SurveyResponse> builder)
        {
            builder.ToTable("survey_response");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.SurveyId).HasColumnName("survey_id");
            builder.Property(x => x.UserId).HasColumnName("user_id");
            builder.Property(x => x.SubmittedAt).HasColumnName("submitted_at");
            builder.Property(x => x.Metadata).HasColumnName("metadata");

            builder.HasMany(x => x.Answers)
                .WithOne(x => x.Response)
                .HasForeignKey(x => x.ResponseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}