namespace BackEnd.Domain.Entities
{
    public class ResponseAnswer
    {
        public long Id { get; set; }
        public long ResponseId { get; set; }
        public long QuestionId { get; set; }
        public long SelectedElementId { get; set; }

        public SurveyResponse Response { get; set; } = null!;
        public SurveyQuestion Question { get; set; } = null!;
        public QuestionElement SelectedElement { get; set; } = null!;
    }
}