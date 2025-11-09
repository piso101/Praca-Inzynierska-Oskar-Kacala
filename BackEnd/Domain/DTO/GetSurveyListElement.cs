using BackEnd.Domain.Entities;

namespace BackEnd.Domain.DTO
{
    public class GetSurveyListElement
    {
        #region Constructor
        public GetSurveyListElement(Survey survey)
        {
            this.Description = survey.Description;
            this.Title = survey.Title;
            this.CreatedAt = survey.CreatedAt;
            this.CreatorName = survey.Creator != null ? survey.Creator.DisplayName ?? "Unknown" : "Unknown";
            this.QuestionCount = survey.Questions.Count();
        }
        #endregion

        #region Properties
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorName { get; set; }
        public int QuestionCount { get; set; }
        #endregion
    }
}
