using BackEnd.Domain.Entities;

namespace BackEnd.Domain.DTO
{
    public class GetSurveyDTO
    {
        #region Constructor
        public GetSurveyDTO(Survey survey) {
            this.Description = survey.Description;
            this.Title = survey.Title;
            this.CreatedAt = survey.CreatedAt;
            this.PublishedAt = survey.PublishedAt;
            this.Creator = survey.Creator;
            this.Questions = survey.Questions;
        }
        #endregion

        #region Properties
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public UserAccount? Creator { get; set; }
        public ICollection<SurveyQuestion> Questions { get; set; } = new List<SurveyQuestion>();
        #endregion
    }
}
