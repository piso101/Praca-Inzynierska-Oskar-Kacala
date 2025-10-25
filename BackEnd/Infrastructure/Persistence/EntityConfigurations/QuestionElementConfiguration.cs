using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Infrastructure.Persistence.EntityConfigurations
{
    public class QuestionElementConfiguration : IEntityTypeConfiguration<QuestionElement>
    {
        public void Configure(EntityTypeBuilder<QuestionElement> builder)
        {
            builder.ToTable("question_element");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.QuestionId).HasColumnName("question_id");
            builder.Property(x => x.X).HasColumnName("x").HasColumnType("numeric(10,4)");
            builder.Property(x => x.Y).HasColumnName("y").HasColumnType("numeric(10,4)");
            builder.Property(x => x.Rotation).HasColumnName("rotation").HasColumnType("numeric(10,4)");
            builder.Property(x => x.Scale).HasColumnName("scale").HasColumnType("numeric(10,4)");
            builder.Property(x => x.SvgName).HasColumnName("svg_name");
            builder.Property(x => x.OrderIndex).HasColumnName("order_index");

            builder.HasIndex(x => new { x.QuestionId, x.OrderIndex }).IsUnique();
        }
    }
}


