using BackEnd.Domain.DTO;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BackEndPortfolioSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyController : ControllerBase
    {
        #region Create Survey Endpoint
        [HttpPost("create-survey")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSurvey([FromBody] CreateSurveyDTO request, [FromServices] IRepository<BackEnd.Domain.Entities.Survey> surveyRepo, [FromServices] IRepository<BackEnd.Domain.Entities.UserAccount> userRepo)
        {
            #region UserId from Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Nie można pobrać ID użytkownika z tokenu");
            }
            #endregion

            #region Validations
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required");
            }
            #endregion

            #region Survey Creation and Persistence
            var survey = new BackEnd.Domain.Entities.Survey
            {
                Title = request.Title,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                Creator = userRepo.GetByIdAsync(userId).Result
            };

            await surveyRepo.AddAsync(survey);
            await surveyRepo.SaveChangesAsync();
            #endregion

            return Ok(new { Success = true, SurveyId = survey.Id });
        }
        #endregion

        #region Read Survey Endpoint
        [HttpGet("read-survey")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSurvey([FromQuery] long surveyId, [FromServices] IRepository<BackEnd.Domain.Entities.Survey> surveyRepo, [FromServices] IRepository<BackEnd.Domain.Entities.UserAccount> userRepo)
        {
            #region Load Survey
            var survey = await surveyRepo.GetByIdAsync(surveyId);
            if (survey != null && survey.CreatedBy.HasValue)
            {
                survey.Creator = await userRepo.GetByIdAsync(survey.CreatedBy.Value);
            }
            #endregion

            #region Validations
            if (survey == null)
            {
                return NotFound("Survey not found");
            }
            if (!survey.IsActive)
            {
                return BadRequest("Survey is not active");
            }
            #endregion

            return Ok(new GetSurveyDTO(survey));
        }
        #endregion

        #region Update Survey Endpoint
        [HttpPost("update-survey")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSurvey([FromBody] UpdateSurveyDTO request, [FromServices] IRepository<BackEnd.Domain.Entities.Survey> surveyRepo, [FromServices] IRepository<BackEnd.Domain.Entities.UserAccount> userRepo)
        {
            #region UserId from Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Nie można pobrać ID użytkownika z tokenu");
            }
            #endregion

            #region Load Survey
            var survey = await surveyRepo.GetByIdAsync(request.Id);
            if (survey != null && survey.CreatedBy.HasValue)
            {
                survey.Creator = await userRepo.GetByIdAsync(survey.CreatedBy.Value);
            }
            #endregion

            #region Validations
            if (survey == null)
            {
                return NotFound("Survey not found");
            }
            if (userId != survey.CreatedBy)
            {
                return Forbid("You are not the creator of this survey");
            }
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required");
            }
            #endregion

            #region Survey Update and Persistence
            survey.Title = request.Title;
            survey.Description = request.Description;
            await surveyRepo.UpdateAsync(survey);
            await surveyRepo.SaveChangesAsync();
            #endregion

            return Ok(new { Success = true });
        }
        #endregion

        #region Delete Survey Endpoint
        [HttpDelete("delete-survey")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSurvey([FromQuery] long surveyId, [FromServices] IRepository<BackEnd.Domain.Entities.Survey> surveyRepo)
        {
            #region UserId from Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Nie można pobrać ID użytkownika z tokenu");
            }
            #endregion

            #region Load Survey
            var survey = await surveyRepo.GetByIdAsync(surveyId);
            #endregion

            #region Validations
            if (survey == null)
            {
                return NotFound("Survey not found");
            }
            if (userId != survey.CreatedBy)
            {
                return Forbid("You are not the creator of this survey");
            }
            #endregion

            await surveyRepo.DeleteAsync(survey);
            await surveyRepo.SaveChangesAsync();
            return Ok(new { Success = true });
        }
        #endregion

        #region Get List of Surveys Endpoint
        [HttpGet("get-list-of-surveys")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListOfSurveys([FromServices] IRepository<BackEnd.Domain.Entities.Survey> surveyRepo, [FromServices] IRepository<BackEnd.Domain.Entities.UserAccount> userRepo)
        {
            #region Load Surveys
            var surveys = await surveyRepo.ListAsync(s => s.IsActive);
            foreach (var survey in surveys)
            {
                if (survey.CreatedBy.HasValue)
                {
                    survey.Creator = await userRepo.GetByIdAsync(survey.CreatedBy.Value);
                }
            }
            var surveyDTOs = surveys
                .Where(s=>s.IsActive)
                .Select(s => new GetSurveyListElement(s))
                .ToList();
            #endregion

            #region Validation
            if (!surveyDTOs.Any())
            {
                return NotFound("Survey's not found");
            }
            #endregion

            return Ok(surveyDTOs);
        }
        #endregion
    }
}
