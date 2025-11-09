using BackEnd.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        #region Create Question Endpoint
        [HttpPost("create-question")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateQuestion([FromBody] Domain.DTO.CreateSurveyQuestion request, [FromServices] Domain.Repositories.IRepository<Domain.Entities.SurveyQuestion> questionRepo, [FromServices] Domain.Repositories.IRepository<Domain.Entities.Survey>surveyRepo )
        {
            #region Validations
            if (string.IsNullOrWhiteSpace(request.QuestionText))
            {
                return BadRequest(new { Success = false, Message = "Question text is required" });
            }
            #endregion

            #region Question Creation

            var survey = await surveyRepo.GetByIdAsync(request.SurveyId);
            if (survey == null)
                throw new Exception($"Survey with id {request.SurveyId} not found");

            var question = new Domain.Entities.SurveyQuestion
            {
                SurveyId = survey.Id,
                QuestionText = request.QuestionText,
                OrderIndex = (survey.Questions?.Count ?? 0) + 1,
                CreatedAt = DateTime.UtcNow,
                Survey = survey
            };

            await questionRepo.AddAsync(question);
            await questionRepo.SaveChangesAsync();
            #endregion

            return Ok(new { Success = true, QuestionId = question.Id });
        }
        #endregion

        #region Delete Question Endpoint
        [HttpDelete("delete-question")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuestion([FromQuery] long questionId, [FromServices] Domain.Repositories.IRepository<Domain.Entities.SurveyQuestion> questionRepo)
        {
            var question = await questionRepo.GetByIdAsync(questionId);

            #region Validations
            if (question == null)
            {
                return NotFound(new { Success = false, Message = "Question not found" });
            }
            #endregion

            #region Delete Question
            await questionRepo.DeleteAsync(question);
            await questionRepo.SaveChangesAsync();
            #endregion

            return Ok(new { Success = true, Message = "Question deleted successfully" });
        }
        #endregion

        #region Add Element to Questions Endpoint
        [HttpPost("add-element")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddElementToQuestion([FromBody] Domain.DTO.AddQuestionElementDTO request, 
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.QuestionElement> elementRepo, 
            [FromServices] IRepository<BackEnd.Domain.Entities.Survey> surveyRepo, 
            [FromServices] IRepository<BackEnd.Domain.Entities.UserAccount> userRepo, 
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.SurveyQuestion> questionRepo)
        {
            #region Validations
            if (string.IsNullOrWhiteSpace(request.Rotation.ToString()))
            {
                return BadRequest(new { Success = false, Message = "Element rotation is required" });
            }
            if (string.IsNullOrWhiteSpace(request.Scale.ToString()))
            {
                return BadRequest(new { Success = false, Message = "Element scale is required" });
            }
            if (string.IsNullOrWhiteSpace(request.OrderIndex.ToString()))
            {
                return BadRequest(new { Success = false, Message = "Element Order is required" });
            }
            if(string.IsNullOrWhiteSpace(request.SvgName.ToString()))
            {
                return BadRequest(new { Success = false, Message = "Element SVG Name is required" });
            }
            #endregion

            #region Element Creation
            var element = new Domain.Entities.QuestionElement
            {
                QuestionId = request.QuestionId,
                Rotation = request.Rotation,
                Scale = request.Scale,
                OrderIndex = request.OrderIndex,
                SvgName = request.SvgName,
                Question = await questionRepo.GetByIdAsync(request.QuestionId)
                    ?? throw new Exception($"Question with id {request.QuestionId} not found"),
                X = request.X,
                Y = request.Y
            };

            await elementRepo.AddAsync(element);
            await elementRepo.SaveChangesAsync();
            #endregion

            return Ok(new { Success = true, ElementId = element.Id });
        }
        #endregion
    }
}
