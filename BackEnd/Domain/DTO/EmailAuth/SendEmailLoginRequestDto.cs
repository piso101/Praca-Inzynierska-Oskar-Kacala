namespace BackEnd.Domain.DTO.EmailAuth
{
    public class SendEmailLoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public long? SurveyId { get; set; } // Optional: ID ankiety do której użytkownik jest zapraszany
    }
}

