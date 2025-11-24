namespace BackEnd.Domain.Entities
{
    public class SurveyResponse
    {
        public long Id { get; set; }
        public long SurveyId { get; set; }
        public long UserId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public ResponseAnswer Answers { get; set; } = new ResponseAnswer();
    }
}