using BackEnd.Domain.Entities;

namespace BackEnd.Domain.DTO
{
    public class GetSurveyQuestionListElementDTO
    {
        public GetSurveyQuestionListElementDTO(SurveyQuestion surveyQuestion)
        {
            Id = surveyQuestion.Id;
            SurveyId = surveyQuestion.SurveyId;
            QuestionText = surveyQuestion.QuestionText;
            OrderIndex = surveyQuestion.OrderIndex;
            CreatedAt = surveyQuestion.CreatedAt;
        }

        public long Id { get; set; }
        public long SurveyId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}