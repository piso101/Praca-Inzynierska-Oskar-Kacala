namespace BackEnd.Domain.DTO
{
    public class SubmitSurveyResponseResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long? ResponseId { get; set; }
    }
}

