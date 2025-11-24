using BackEnd.Domain.Entities;

namespace BackEnd.Domain.DTO
{
    public class UpdateSurveyQuestionDTO
    {
        public long Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
    }
}
