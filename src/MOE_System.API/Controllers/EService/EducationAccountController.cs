using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.EService.Interfaces.Services;

namespace MOE_System.API.Controllers.EService
{
    // 87c34001-5065-49d6-bb29-b578fd1532a1
    [Route("api/v1/education-accounts")]
    [ApiController]
    public class EducationAccountController : ControllerBase
    {
        private readonly IEducationAccountService _educationAccountService;
        private readonly IEnrollmentService _enrollmentService;
        public EducationAccountController(IEducationAccountService educationAccountService, IEnrollmentService enrollmentService)
        {
            _educationAccountService = educationAccountService;
            _enrollmentService = enrollmentService;
        }

        [HttpGet("{accountId}/balance")]
        public async Task<IActionResult> GetEducationAccountBalance([FromRoute] string accountId)
        {
            try
            {
                var res = await _educationAccountService.GetEducationAccountBalanceAsync(accountId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { Error = message, StackTrace = ex.ToString() });

            }

        }

        [HttpGet("{accountId}/outstanding-fees")]
        public async Task<IActionResult> GetOutstandingFees([FromRoute] string accountId)
        {
            try
            {
                var res = await _enrollmentService.GetOutstandingFeeAsync(accountId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { Error = message, StackTrace = ex.ToString() });

            }

        }
    }
}
