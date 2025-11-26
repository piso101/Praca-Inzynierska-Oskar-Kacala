using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Infrastructure.Persistence.EntityConfigurations
{
    public class QuestAccountConfiguration : IEntityTypeConfiguration<QuestAccount>
    {
        public void Configure(EntityTypeBuilder<QuestAccount> builder)
        {
            builder.ToTable("quest_account");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Email).HasColumnName("email").IsRequired();
            builder.Property(x => x.DisplayName).HasColumnName("display_name");
            builder.Property(x => x.Role).HasColumnName("role");
            builder.Property(x => x.IsActive).HasColumnName("is_active");
            builder.Property(x => x.CreatedAt).HasColumnName("created_at");
            builder.Property(x => x.EmailVerificationToken).HasColumnName("email_verification_token");
            builder.Property(x => x.TokenExpiresAt).HasColumnName("token_expires_at");
            builder.Property(x => x.EmailVerifiedAt).HasColumnName("email_verified_at");

            builder.HasIndex(x => x.Email);
            builder.HasIndex(x => x.EmailVerificationToken);
        }
    }
}


