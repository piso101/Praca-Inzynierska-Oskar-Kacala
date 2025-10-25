namespace BackEnd.Domain.Entities
{
    public class SurveyQuestion
    {
        public long Id { get; set; }
        public long SurveyId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }

        public Survey Survey { get; set; } = null!;
        public ICollection<QuestionElement> Elements { get; set; } = new List<QuestionElement>();
    }
}


