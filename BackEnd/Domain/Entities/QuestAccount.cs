using BackEnd.Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Domain.Entities
{
    public class QuestAccount
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public UserRole Role { get; set; } = UserRole.Quest;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? EmailVerificationToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }

        [NotMapped]
        public ICollection<int> AnsweredSurveysIds { get; set; } = new List<int>();
    }
}
