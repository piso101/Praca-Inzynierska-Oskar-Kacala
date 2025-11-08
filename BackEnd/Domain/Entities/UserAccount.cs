using BackEnd.Domain.Shared;

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

        public ICollection<Survey> CreatedSurveys { get; set; } = new List<Survey>();
        public ICollection<SurveyResponse> Responses { get; set; } = new List<SurveyResponse>();
    }
}


