using BackEnd.Domain.Entities;

namespace BackEnd.Domain.DTO
{
    public class UpdateSurveyDTO
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
