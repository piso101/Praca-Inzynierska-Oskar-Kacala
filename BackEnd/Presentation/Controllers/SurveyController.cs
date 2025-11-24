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
        private readonly IInviteCodeService _inviteCodeService;

        public SurveyController(IInviteCodeService inviteCodeService)
        {
            _inviteCodeService = inviteCodeService;
        }
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
        [Authorize(Roles = "Admin,Quest")]
        public async Task<IActionResult> GetSurvey([FromQuery] long surveyId, [FromServices] IRepository<BackEnd.Domain.Entities.Survey> surveyRepo
            , [FromServices] IRepository<BackEnd.Domain.Entities.UserAccount> userRepo
            , [FromServices] BackEnd.Domain.Repositories.IRepository<BackEnd.Domain.Entities.QuestionElement> elementRepo,
            [FromServices] BackEnd.Domain.Repositories.IRepository<BackEnd.Domain.Entities.SurveyQuestion> questionRepo)
        {
            #region Load Survey
            var survey = await surveyRepo.GetByIdAsync(surveyId);
            
            #region Validations
            if (survey == null)
            {
                return NotFound("Survey not found");
            }
            #endregion

            if (survey.CreatedBy.HasValue)
            {
                survey.Creator = await userRepo.GetByIdAsync(survey.CreatedBy.Value);
            }
            survey.Questions = (await questionRepo.ListAsync(q => q.SurveyId == surveyId)).ToList();

            foreach (var question in survey.Questions)
            {
                question.Elements = (await elementRepo.ListAsync(e => e.QuestionId == question.Id)).ToList();
            }
            #endregion

            #region Additional Validations
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

        #region Get Survey inviteCode Endpoint
        [HttpGet("get-survey-invite-code")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetSurveyInviteCode([FromQuery] long surveyId)
        {
            string inviteCode = _inviteCodeService.GenerateInviteCode(surveyId);

            return Ok(new { Success = true, InviteCode = inviteCode });
        }
        #endregion

        #region Submit Survey Response Endpoint
        [HttpPost("submit-response")]
        [Authorize(Roles = "Quest")]
        public async Task<ActionResult<SubmitSurveyResponseResultDto>> SubmitSurveyResponse(
            [FromBody] SubmitSurveyResponseDto request, 
            [FromServices] IRepository<SurveyResponse> responseRepo,
            [FromServices] IRepository<ResponseAnswer> answerRepo,
            [FromServices] IRepository<Survey> surveyRepo)
        {
            #region UserId from Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new SubmitSurveyResponseResultDto 
                { 
                    Success = false, 
                    Message = "Cannot get user ID from token" 
                });
            }
            #endregion

            #region Validations
            var survey = await surveyRepo.GetByIdAsync(request.SurveyId);
            if (survey == null)
            {
                return NotFound(new SubmitSurveyResponseResultDto 
                { 
                    Success = false, 
                    Message = "Survey not found" 
                });
            }

            if (!survey.IsActive)
            {
                return BadRequest(new SubmitSurveyResponseResultDto 
                { 
                    Success = false, 
                    Message = "Survey is not active" 
                });
            }

            if (request.Answers == null || !request.Answers.Any())
            {
                return BadRequest(new SubmitSurveyResponseResultDto 
                { 
                    Success = false, 
                    Message = "No answers provided" 
                });
            }
            #endregion

            #region Create Survey Response
            var surveyResponse = new SurveyResponse
            {
                SurveyId = request.SurveyId,
                UserId = userId,
                SubmittedAt = DateTime.UtcNow
            };

            await responseRepo.AddAsync(surveyResponse);
            await responseRepo.SaveChangesAsync();
            #endregion

            #region Save Answers
            foreach (var answer in request.Answers)
            {
                var responseAnswer = new ResponseAnswer
                {
                    ResponseId = surveyResponse.Id,
                    QuestionId = answer.Key,
                    SelectedElementId = answer.Value
                };
                await answerRepo.AddAsync(responseAnswer);
            }
            await answerRepo.SaveChangesAsync();
            #endregion

            return Ok(new SubmitSurveyResponseResultDto 
            { 
                Success = true, 
                Message = "Survey response submitted successfully",
                ResponseId = surveyResponse.Id
            });
        }
        #endregion
    }
}
