namespace BackEnd.Domain.Entities
{
    public class SurveyResponse
    {
        public long Id { get; set; }
        public long SurveyId { get; set; }
        public long? UserId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? Metadata { get; set; }

        public Survey Survey { get; set; } = null!;
        public UserAccount? User { get; set; }
        public ICollection<ResponseAnswer> Answers { get; set; } = new List<ResponseAnswer>();
    }
}