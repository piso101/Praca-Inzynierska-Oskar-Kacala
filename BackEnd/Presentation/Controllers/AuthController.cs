using BackEnd.Application.Contracts.Auth;
using BackEnd.Domain.Repositories;
using BackEnd.Domain.Shared;
using BackEnd.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEndPortfolioSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        #region Health Check Endpoint
        [HttpGet("health")]
        [Authorize(Roles = "Admin")]
        public IActionResult Health() => Ok("API is running!");
        #endregion

        #region Login Endpoint
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

            if (user.PasswordHash != request.Password)
            {
                return Unauthorized(new LoginResponseDto { Success = false, Message = "Invalid credentials" });
            }

            var token = tokens.GenerateToken(user);
            return Ok(new LoginResponseDto { Success = true, Token = token });
        }
        #endregion

        #region Create Admin Endpoint
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
        #endregion

        #region Get Current User Endpoint
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser([FromServices] IUserRepository users)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Nie można pobrać ID użytkownika z tokenu");
            }

            var user = await users.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                user.Email,
                user.DisplayName,
                user.Role,
                user.IsActive
            });
        }
        #endregion
    }
}