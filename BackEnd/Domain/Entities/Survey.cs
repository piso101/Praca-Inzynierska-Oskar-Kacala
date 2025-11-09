namespace BackEnd.Domain.Entities
{
    public class Survey
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public long? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public UserAccount? Creator { get; set; }
        public ICollection<SurveyQuestion> Questions { get; set; } = new List<SurveyQuestion>();
    }
}