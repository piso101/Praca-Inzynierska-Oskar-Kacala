using BackEnd.Application.Contracts.Auth;
using BackEnd.Domain.Repositories;
using BackEnd.Domain.Shared;
using BackEnd.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndPortfolioSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("health")]
        [Authorize(Roles = "Admin")]
        public IActionResult Health() => Ok("API is running!");

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request, [FromServices] IUserRepository users, [FromServices] JwtTokenService tokens)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new LoginResponseDto { Success = false, Message = "Email and password required" });
            }

            var user = await users.GetByEmailAsync(request.Email);
            if (user == null || user.Role != UserRole.Admin || !user.IsActive)
            {
                return Unauthorized(new LoginResponseDto { Success = false, Message = "Invalid credentials or not admin" });
            }

            // Wersja uproszczona: hasz jest przechowywany w PasswordHash i porównywany z plain (do wymiany na PBKDF2/BCrypt)
            if (user.PasswordHash != request.Password)
            {
                return Unauthorized(new LoginResponseDto { Success = false, Message = "Invalid credentials" });
            }

            var token = tokens.GenerateToken(user);
            return Ok(new LoginResponseDto { Success = true, Token = token });
        }

        [HttpPost("create-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequestDto request, [FromServices] IUserRepository users, [FromServices] IRepository<BackEnd.Domain.Entities.UserAccount> repo, [FromServices] IConfiguration config)
        {
            var ownerPassword = config["OwnerPassword"];

            #region Validations
            if (string.IsNullOrWhiteSpace(ownerPassword) || request.OwnerPassword != ownerPassword)
            {
                return Unauthorized("Invalid owner password");
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password required");
            }

            var existing = await users.GetByEmailAsync(request.Email);
            if (existing != null)
            {
                return Conflict("User already exists");
            }
            #endregion

            var user = new BackEnd.Domain.Entities.UserAccount
            {
                Email = request.Email,
                PasswordHash = request.Password,
                DisplayName = request.DisplayName ?? request.Email,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(user);
            await repo.SaveChangesAsync();
            return Ok();
        }
    }
}