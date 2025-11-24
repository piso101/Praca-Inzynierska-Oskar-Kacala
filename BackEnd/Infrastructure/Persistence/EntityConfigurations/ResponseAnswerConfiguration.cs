using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Infrastructure.Persistence.EntityConfigurations
{
    public class ResponseAnswerConfiguration : IEntityTypeConfiguration<ResponseAnswer>
    {
        public void Configure(EntityTypeBuilder<ResponseAnswer> builder)
        {
            builder.ToTable("response_answer");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ResponseId).HasColumnName("response_id");
        }
    }
}