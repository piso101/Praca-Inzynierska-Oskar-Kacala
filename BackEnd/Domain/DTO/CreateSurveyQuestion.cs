using BackEnd.Domain.Entities;

namespace BackEnd.Domain.DTO
{
    public class CreateSurveyQuestion
    {
        public long SurveyId { get; set; }
        public string QuestionText { get; set; } = "Default Name";
    }
}
