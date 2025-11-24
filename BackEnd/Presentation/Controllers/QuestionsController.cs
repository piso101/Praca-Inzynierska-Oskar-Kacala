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
        public async Task<IActionResult> CreateQuestion([FromBody] Domain.DTO.CreateSurveyQuestion request, [FromServices] Domain.Repositories.IRepository<Domain.Entities.SurveyQuestion> questionRepo, [FromServices] Domain.Repositories.IRepository<Domain.Entities.Survey> surveyRepo)
        {
            #region Validations

            if (string.IsNullOrWhiteSpace(request.QuestionText))
            {
                return BadRequest(new { Success = false, Message = "Question text is required" });
            }

            #endregion Validations

            #region Question Creation

            var survey = await surveyRepo.GetByIdAsync(request.SurveyId);
            if (survey == null)
                throw new Exception($"Survey with id {request.SurveyId} not found");

            survey.Questions = (await questionRepo.ListAsync(q => q.SurveyId == survey.Id)).ToList();

            var question = new Domain.Entities.SurveyQuestion
            {
                Id = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000L) + Random.Shared.Next(0, 1000),
                SurveyId = survey.Id,
                QuestionText = request.QuestionText,
                OrderIndex = (survey.Questions?.Count ?? 0) + 1,
                CreatedAt = DateTime.UtcNow,
                Survey = survey
            };

            await questionRepo.AddAsync(question);
            await questionRepo.SaveChangesAsync();

            #endregion Question Creation

            return Ok(new { Success = true, QuestionId = question.Id });
        }

        #endregion Create Question Endpoint

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

            #endregion Validations

            #region Delete Question

            await questionRepo.DeleteAsync(question);
            await questionRepo.SaveChangesAsync();

            #endregion Delete Question

            return Ok(new { Success = true, Message = "Question deleted successfully" });
        }

        #endregion Delete Question Endpoint

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
            if (string.IsNullOrWhiteSpace(request.SvgName.ToString()))
            {
                return BadRequest(new { Success = false, Message = "Element SVG Name is required" });
            }

            #endregion Validations

            #region Element Creation

            var Question = await questionRepo.GetByIdAsync(request.QuestionId);
            if (Question == null)
            {
                return NotFound(new { Success = false, Message = $"Question with id {request.QuestionId} not found" });
            }

            Question.Elements = (await elementRepo.ListAsync(e => e.QuestionId == Question.Id)).ToList();

            var element = new Domain.Entities.QuestionElement
            {
                QuestionId = request.QuestionId,
                Rotation = request.Rotation,
                Scale = request.Scale,
                OrderIndex = Question?.Elements.Count() == null ? 9 : Question.Elements.Count() + 1,
                SvgName = request.SvgName,
                Question = await questionRepo.GetByIdAsync(request.QuestionId)
                    ?? throw new Exception($"Question with id {request.QuestionId} not found"),
                X = request.X,
                Y = request.Y
            };

            await elementRepo.AddAsync(element);
            await elementRepo.SaveChangesAsync();

            #endregion Element Creation

            return Ok(new { Success = true, ElementId = element.Id });
        }

        #endregion Add Element to Questions Endpoint

        #region Read List of Questions for survey

        [HttpGet("question")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddElementToQuestion([FromQuery] long surveyId,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.QuestionElement> elementRepo,
            [FromServices] IRepository<BackEnd.Domain.Entities.Survey> surveyRepo,
            [FromServices] IRepository<BackEnd.Domain.Entities.UserAccount> userRepo,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.SurveyQuestion> questionRepo)
        {
            #region Validations

            if (string.IsNullOrWhiteSpace(surveyId.ToString()))
            {
                return BadRequest(new { Success = false, Message = "ID cannot be null" });
            }

            #endregion Validations

            #region Question Read

            var survey = await surveyRepo.GetByIdAsync(surveyId);
            if (survey == null)
                throw new Exception($"Survey with id {surveyId} not found");

            survey.Questions = (await questionRepo.ListAsync(q => q.SurveyId == surveyId)).ToList();
            var questions = new List<Domain.DTO.GetSurveyQuestionListElementDTO>();
            foreach (var element in survey.Questions)
            {
                questions.Add(new Domain.DTO.GetSurveyQuestionListElementDTO(element));
            }

            return Ok(questions);

            #endregion Question Read
        }

        #endregion Read List of Questions for survey

        #region Update Question list order
        [HttpPut("update-question-order")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateQuestionOrder([FromBody] Domain.DTO.UpdateQuestionOrderDTO request,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.SurveyQuestion> questionRepo)
        {
            #region Validations
            if (request.Questions == null || !request.Questions.Any())
            {
                return BadRequest(new { Success = false, Message = "Question order list cannot be empty" });
            }
            #endregion Validations

            #region Update OrderIds so that they match the provided order
            foreach (var item in request.Questions)
            {
                var question = await questionRepo.GetByIdAsync(item.Item1);
                if (question != null)
                {
                    question.OrderIndex = item.Item2+99999;
                    await questionRepo.UpdateAsync(question);
                }
            }
            await questionRepo.SaveChangesAsync();
            #endregion Update OrderIds so that they match the provided order

            #region Update Order
            foreach (var item in request.Questions)
            {
                var question = await questionRepo.GetByIdAsync(item.Item1);
                if (question != null)
                {
                    question.OrderIndex = item.Item2;
                    await questionRepo.UpdateAsync(question);
                }
            }
            await questionRepo.SaveChangesAsync();
            #endregion Update Order

            return Ok(new { Success = true, Message = "Question order updated successfully" });
        }
        #endregion Update Question list order

        #region Update Question
        [HttpPut("update-question")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateQuestion([FromQuery] Domain.DTO.UpdateSurveyQuestionDTO request,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.SurveyQuestion> questionRepo)
        {
            #region Validations
            if (string.IsNullOrWhiteSpace(request.QuestionText))
            {
                return BadRequest(new { Success = false, Message = "Question text is required" });
            }
            #endregion Validations
            #region Update Question
            var question = await questionRepo.GetByIdAsync(request.Id);
            if (question == null)
            {
                return NotFound(new { Success = false, Message = "Question not found" });
            }
            question.QuestionText = request.QuestionText;
            await questionRepo.UpdateAsync(question);
            await questionRepo.SaveChangesAsync();
            #endregion Update Question
            return Ok(new { Success = true, Message = "Question updated successfully" });
        }
        #endregion Update Question

        #region Read Question List Of Elements 
        [HttpGet("question-elements")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetQuestionElements([FromQuery] long questionId,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.QuestionElement> elementRepo)
        {
            #region Validations
            if (string.IsNullOrWhiteSpace(questionId.ToString()))
            {
                return BadRequest(new { Success = false, Message = "Question ID cannot be null" });
            }
            #endregion Validations
            #region Element Read
            var elements = await elementRepo.ListAsync(e => e.QuestionId == questionId);
            var elementDtos = elements.Select(e => new Domain.DTO.QuestionListElementDTO(e)).ToList();
            return Ok(elementDtos);
            #endregion Element Read
        }
        #endregion Read Question List Of Elements

        #region Delete Question Element
        [HttpDelete("delete-question-element")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuestionElement([FromQuery] long elementId,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.QuestionElement> elementRepo)
        {
            #region Validations
            var element = await elementRepo.GetByIdAsync(elementId);
            if (element == null)
            {
                return NotFound(new { Success = false, Message = "Element not found" });
            }
            #endregion Validations
            #region Delete Element
            await elementRepo.DeleteAsync(element);
            await elementRepo.SaveChangesAsync();
            #endregion Delete Element
            return Ok(new { Success = true, Message = "Element deleted successfully" });
        }
        #endregion Delete Question Element

        #region update Question Element
        [HttpPut("update-question-element")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateQuestionElement([FromBody] Domain.Entities.QuestionElement request,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.QuestionElement> elementRepo)
        {
            #region Validations
            var element = await elementRepo.GetByIdAsync(request.Id);
            if (element == null)
            {
                return NotFound(new { Success = false, Message = "Element not found" });
            }
            #endregion Validations
            #region Update Element
            element.X = request.X;
            element.Y = request.Y;
            element.Rotation = request.Rotation;
            element.Scale = request.Scale;
            element.OrderIndex = request.OrderIndex;
            element.SvgName = request.SvgName;
            await elementRepo.UpdateAsync(element);
            await elementRepo.SaveChangesAsync();
            #endregion Update Element
            return Ok(new { Success = true, Message = "Element updated successfully" });
        }
        #endregion update Question Element

        #region get Question Element by Id
        [HttpGet("question-element")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetQuestionElementById([FromQuery] long elementId,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.QuestionElement> elementRepo)
        {
            #region Validations
            var element = await elementRepo.GetByIdAsync(elementId);
            if (element == null)
            {
                return NotFound(new { Success = false, Message = "Element not found" });
            }
            #endregion Validations
            #region Return Element
            
            return Ok(element);
            #endregion Return Element
        }
        #endregion get Question Element by Id

        #region Change Question element order
        [HttpPut("change-element-order")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeQuestionElementOrder([FromBody] Domain.DTO.ChangeQuestionElementOrderDTO request,
            [FromServices] Domain.Repositories.IRepository<Domain.Entities.QuestionElement> elementRepo)
        {
            #region Validations
            if (request.Elements == null || !request.Elements.Any())
            {
                return BadRequest(new { Success = false, Message = "Element order list cannot be empty" });
            }
            #endregion Validations
            #region Update OrderIds so that they match the provided order
            foreach (var item in request.Elements)
            {
                var element = await elementRepo.GetByIdAsync(item.Item1);
                if (element != null)
                {
                    element.OrderIndex = item.Item2 + 99999;
                    await elementRepo.UpdateAsync(element);
                }
            }
            await elementRepo.SaveChangesAsync();
            #endregion Update OrderIds so that they match the provided order
            #region Update Order
            foreach (var item in request.Elements)
            {
                var element = await elementRepo.GetByIdAsync(item.Item1);
                if (element != null)
                {
                    element.OrderIndex = item.Item2;
                    await elementRepo.UpdateAsync(element);
                }
            }
            await elementRepo.SaveChangesAsync();
            #endregion Update Order
            return Ok(new { Success = true, Message = "Element order updated successfully" });
        }
        #endregion Change Question element order
    }
}