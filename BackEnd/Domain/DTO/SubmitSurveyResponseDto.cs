namespace BackEnd.Domain.DTO
{
    public class SubmitSurveyResponseDto
    {
        public long SurveyId { get; set; }
        public Dictionary<long, long> Answers { get; set; } = new Dictionary<long, long>(); // QuestionId -> ElementId
    }
}

