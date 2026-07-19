using BackEnd.Domain.DTO.EmailAuth;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Repositories;
using BackEnd.Domain.Shared;
using BackEnd.Infrastructure.Services;
using BackEnd.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEndPortfolioSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailAuthController : ControllerBase
    {
        private readonly IQuestAccountRepository _questAccountRepo;
        private readonly IEmailSenderService _emailService;
        private readonly JwtTokenService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailAuthController> _logger;

        public EmailAuthController(
            IQuestAccountRepository questAccountRepo,
            IEmailSenderService emailService,
            JwtTokenService jwtService,
            IConfiguration configuration,
            ILogger<EmailAuthController> logger)
        {
            _questAccountRepo = questAccountRepo;
            _emailService = emailService;
            _jwtService = jwtService;
            _configuration = configuration;
            _logger = logger;
        }

        #region Send Email Login Link Endpoint
        [HttpPost("send-login-email")]
        [AllowAnonymous]
        public async Task<ActionResult<SendEmailLoginResponseDto>> SendLoginEmail(
            [FromBody] SendEmailLoginRequestDto request,
            [FromServices] IRepository<Survey> surveyRepo,
            [FromServices] IRepository<SurveyResponse> responseRepo)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new SendEmailLoginResponseDto 
                { 
                    Success = false, 
                    Message = "Email is required" 
                });
            }

            var questAccount = await _questAccountRepo.GetByEmailAsync(request.Email);
            
            if (request.SurveyId.HasValue && questAccount != null)
            {
                var existingResponses = await responseRepo.ListAsync(r => 
                    r.SurveyId == request.SurveyId.Value && 
                    r.QuestAccountId == questAccount.Id);
                
                if (existingResponses.Any())
                {
                    return BadRequest(new SendEmailLoginResponseDto 
                    { 
                        Success = false, 
                        Message = "Już odpowiedziałeś na tę ankietę. Każdy użytkownik może odpowiedzieć tylko raz." 
                    });
                }
            }
            
            var token = Guid.NewGuid().ToString();
            
            if (questAccount == null)
            {
                questAccount = new QuestAccount
                {
                    Email = request.Email,
                    DisplayName = request.Email.Split('@')[0],
                    Role = UserRole.Quest,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailVerificationToken = token,
                    TokenExpiresAt = DateTime.UtcNow.AddHours(1)
                };
                await _questAccountRepo.AddAsync(questAccount);
                await _questAccountRepo.SaveChangesAsync();
            }
            else
            {
                questAccount.EmailVerificationToken = token;
                questAccount.TokenExpiresAt = DateTime.UtcNow.AddHours(1);
                await _questAccountRepo.UpdateAsync(questAccount);
                await _questAccountRepo.SaveChangesAsync();
            }

            string surveyTitle = "Ankieta";
            string surveyDescription = "Prosimy o wypełnienie ankiety.";
            
            if (request.SurveyId.HasValue)
            {
                var survey = await surveyRepo.GetByIdAsync(request.SurveyId.Value);
                if (survey != null)
                {
                    surveyTitle = survey.Title;
                    surveyDescription = survey.Description ?? "Prosimy o wypełnienie poniższej ankiety.";
                }
            }

            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
            var loginLink = $"{frontendUrl}/email-login?token={token}";
            
            if (request.SurveyId.HasValue)
            {
                loginLink += $"&returnUrl=/public/survey/{request.SurveyId}/start";
            }

            var emailSent = await _emailService.SendSurveyInvitationEmailAsync(
                request.Email,
                questAccount.DisplayName ?? "Użytkowniku",
                surveyTitle,
                surveyDescription,
                loginLink
            );

            if (!emailSent)
            {
                _logger.LogError($"Failed to send email to {request.Email}");
                return StatusCode(500, new SendEmailLoginResponseDto 
                { 
                    Success = false, 
                    Message = "Failed to send email" 
                });
            }

            return Ok(new SendEmailLoginResponseDto 
            { 
                Success = true, 
                Message = "Email wysłany pomyślnie. Sprawdź swoją skrzynkę pocztową." 
            });
        }
        #endregion

        #region Verify Email Login Token Endpoint
        [HttpGet("verify-login")]
        [AllowAnonymous]
        public async Task<ActionResult<VerifyEmailLoginResponseDto>> VerifyLogin([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new VerifyEmailLoginResponseDto 
                { 
                    Success = false, 
                    Message = "Token is required" 
                });
            }

            var questAccount = await _questAccountRepo.GetByVerificationTokenAsync(token);
            if (questAccount == null)
            {
                return Unauthorized(new VerifyEmailLoginResponseDto 
                { 
                    Success = false, 
                    Message = "Invalid or expired token" 
                });
            }

            questAccount.EmailVerifiedAt = DateTime.UtcNow;
            questAccount.EmailVerificationToken = null;
            questAccount.TokenExpiresAt = null;

            await _questAccountRepo.UpdateAsync(questAccount);
            await _questAccountRepo.SaveChangesAsync();

            var jwtToken = _jwtService.GenerateQuestToken(questAccount);

            return Ok(new VerifyEmailLoginResponseDto 
            { 
                Success = true, 
                Token = jwtToken,
                Message = "Login successful" 
            });
        }
        #endregion

        #region Get Current Quest User Endpoint
        [HttpGet("me")]
        [Authorize(Roles = "Quest")]
        public async Task<IActionResult> GetCurrentQuestUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Cannot get user ID from token");
            }

            var questAccount = await _questAccountRepo.GetByIdAsync(userId);
            if (questAccount == null)
            {
                return NotFound("Quest account not found");
            }

            return Ok(new
            {
                questAccount.Email,
                questAccount.DisplayName,
                questAccount.Role,
                questAccount.IsActive
            });
        }
        #endregion

    }
}
