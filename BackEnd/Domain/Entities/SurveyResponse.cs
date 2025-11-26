namespace BackEnd.Domain.Entities
{
    public class SurveyResponse
    {
        public long Id { get; set; }
        public long SurveyId { get; set; }
        public long? UserId { get; set; }  // Admin user (nullable)
        public long? QuestAccountId { get; set; }  // Quest account (nullable)
        public DateTime SubmittedAt { get; set; }
        public ICollection<ResponseAnswer> Answers { get; set; } = new List<ResponseAnswer>();
    }
}