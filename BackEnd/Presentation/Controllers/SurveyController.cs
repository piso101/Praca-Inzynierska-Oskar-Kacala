using BackEnd.Domain.DTO;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace BackEndPortfolioSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyController : ControllerBase
    {
        private readonly IInviteCodeService _inviteCodeService;
        private readonly IConfiguration _configuration;

        public SurveyController(IInviteCodeService inviteCodeService, IConfiguration configuration)
        {
            _inviteCodeService = inviteCodeService;
            _configuration = configuration;
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
            
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isQuest = userRole == "Quest";
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
            
            IReadOnlyList<SurveyResponse> existingResponses;
            
            if (isQuest)
            {
                existingResponses = await responseRepo.ListAsync(r => 
                    r.SurveyId == request.SurveyId && 
                    r.QuestAccountId == userId);
            }
            else
            {
                existingResponses = await responseRepo.ListAsync(r => 
                    r.SurveyId == request.SurveyId && 
                    r.UserId == userId);
            }
            
            if (existingResponses.Any())
            {
                return BadRequest(new SubmitSurveyResponseResultDto 
                { 
                    Success = false, 
                    Message = "Już odpowiedziałeś na tę ankietę. Każdy użytkownik może odpowiedzieć tylko raz." 
                });
            }
            #endregion

            #region Create Survey Response
            var surveyResponse = new SurveyResponse
            {
                SurveyId = request.SurveyId,
                UserId = isQuest ? null : userId,
                QuestAccountId = isQuest ? userId : null,
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

        #region Get Survey Results Endpoint
        [HttpGet("survey-results")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SurveyResultsDTO>> GetSurveyResults(
            [FromQuery] long surveyId,
            [FromServices] IRepository<Survey> surveyRepo,
            [FromServices] IRepository<SurveyQuestion> questionRepo,
            [FromServices] IRepository<QuestionElement> elementRepo,
            [FromServices] IRepository<SurveyResponse> responseRepo,
            [FromServices] IRepository<ResponseAnswer> answerRepo)
        {
            #region Load Survey
            var survey = await surveyRepo.GetByIdAsync(surveyId);
            if (survey == null)
            {
                return NotFound("Survey not found");
            }
            #endregion

            #region Load Questions and Elements
            var questions = (await questionRepo.ListAsync(q => q.SurveyId == surveyId))
                .OrderBy(q => q.OrderIndex)
                .ToList();

            var questionIds = questions.Select(q => q.Id).ToList();
            var allElements = (await elementRepo.ListAsync(e => questionIds.Contains(e.QuestionId)))
                .ToList();
            #endregion

            #region Load Responses and Answers
            var responses = await responseRepo.ListAsync(r => r.SurveyId == surveyId);
            var responseIds = responses.Select(r => r.Id).ToList();
            
            var allAnswers = responseIds.Any() 
                ? (await answerRepo.ListAsync(a => responseIds.Contains(a.ResponseId))).ToList()
                : new List<ResponseAnswer>();
            #endregion

            #region Aggregate Results
            var questionResults = new List<QuestionResultDTO>();

            foreach (var question in questions)
            {
                var questionElements = allElements
                    .Where(e => e.QuestionId == question.Id)
                    .OrderBy(e => e.OrderIndex)
                    .ToList();

                var questionAnswers = allAnswers
                    .Where(a => a.QuestionId == question.Id)
                    .ToList();

                var totalAnswers = questionAnswers.Count;

                var elementResults = questionElements.Select(element =>
                {
                    var selectionCount = questionAnswers.Count(a => a.SelectedElementId == element.Id);
                    var selectionPercentage = totalAnswers > 0 
                        ? Math.Round((double)selectionCount / totalAnswers * 100, 2) 
                        : 0;

                    return new ElementResultDTO
                    {
                        ElementId = element.Id,
                        X = (double)element.X,
                        Y = (double)element.Y,
                        Rotation = (double)element.Rotation,
                        Scale = (double)element.Scale,
                        SvgName = element.SvgName,
                        SelectionCount = selectionCount,
                        SelectionPercentage = selectionPercentage,
                        OrderIndex = element.OrderIndex
                    };
                }).ToList();

                questionResults.Add(new QuestionResultDTO
                {
                    QuestionId = question.Id,
                    QuestionText = question.QuestionText,
                    OrderIndex = question.OrderIndex,
                    TotalAnswers = totalAnswers,
                    Elements = elementResults
                });
            }
            #endregion

            #region Build Response DTO
            var inviteCode = _inviteCodeService.GenerateInviteCode(survey.Id);
            var frontendUrl = _configuration["FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:5173";

            var resultsDto = new SurveyResultsDTO
            {
                SurveyId = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                TotalResponses = responses.Count,
                Questions = questionResults,
                InviteCode = inviteCode,
                ShareLink = $"{frontendUrl}/join/{inviteCode}"
            };
            #endregion

            return Ok(resultsDto);
        }
        #endregion
    }
}
