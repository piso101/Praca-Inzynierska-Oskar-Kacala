namespace BackEnd.Application.Contracts.Auth
{
    public class CreateAdminRequestDto
    {
        public string OwnerPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
    }
}