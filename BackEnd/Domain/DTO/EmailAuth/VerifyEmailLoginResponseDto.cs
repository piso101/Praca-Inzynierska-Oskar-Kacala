namespace BackEnd.Domain.DTO.EmailAuth
{
    public class VerifyEmailLoginResponseDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}


