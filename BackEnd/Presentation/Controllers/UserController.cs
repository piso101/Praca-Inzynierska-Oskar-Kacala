using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Presentation.Controllers
{
    [ApiController]
    [Route("open-survey")]
    public class UserController : ControllerBase
    {
        private readonly IInviteCodeService _inviteCodeService;

        public UserController(IInviteCodeService inviteCodeService)
        {
            _inviteCodeService = inviteCodeService;
        }
        [HttpGet]
        public IActionResult GetSurveyIdByInviteCode([FromQuery] string inviteCode)
        {
            #region Validations
            if (string.IsNullOrWhiteSpace(inviteCode))
            {
                return BadRequest("Invite code is required");
            }
            #endregion
            var surveyid = _inviteCodeService.GetSurveyIdFromInviteCode(inviteCode);
            if (surveyid == null)
            {
                return NotFound("Invalid invite code");
            }

            return Ok(new { Success = true, SurveyId = surveyid });
        }
    }
}
