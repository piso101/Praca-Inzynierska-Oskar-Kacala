namespace BackEnd.Domain.DTO
{
    public class SurveyResultsDTO
    {
        public long SurveyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalResponses { get; set; }
        public string InviteCode { get; set; } = string.Empty;
        public string ShareLink { get; set; } = string.Empty;
        public List<QuestionResultDTO> Questions { get; set; } = new();
    }

    public class QuestionResultDTO
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public int TotalAnswers { get; set; }
        public List<ElementResultDTO> Elements { get; set; } = new();
    }

    public class ElementResultDTO
    {
        public long ElementId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Rotation { get; set; }
        public double Scale { get; set; }
        public string SvgName { get; set; } = string.Empty;
        public int SelectionCount { get; set; }
        public double SelectionPercentage { get; set; }
        public int OrderIndex { get; set; }
    }
}

