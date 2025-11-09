using BackEnd.Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Domain.Entities
{
    public class UserAccount
    {
        public long Id { get; set; }
        public UserRole Role { get; set; } = UserRole.Anonymous;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public ICollection<int> CreatedSurveysIds { get; set; } = new List<int>();
    }
}