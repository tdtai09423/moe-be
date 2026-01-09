using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.EService.Interfaces.Services;

namespace MOE_System.API.Controllers.EService
{
    // 89973440-e26b-41d1-a61e-9b287ebfa4a1
    [Route("api/v1/account-holders")]
    [ApiController]
    public class AccountHolderController : ControllerBase
    {
        private readonly IAccountHolderEServiceService _accountHolderEServiceService;
        private readonly IEnrollmentService _enrollmentService;

        public AccountHolderController(IAccountHolderEServiceService accountHolderEServiceService,
            IEnrollmentService enrollmentService)
        {
            _accountHolderEServiceService = accountHolderEServiceService;
            _enrollmentService = enrollmentService;
        }

        [HttpGet("{accountHolderId}")]
        public async Task<IActionResult> GetAccountHolder([FromRoute] string accountHolderId)
        {
            try
            {
                var res = await _accountHolderEServiceService.GetAccountHolderInformationAsync(accountHolderId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { Error = message, StackTrace = ex.ToString() });

            }

        }

        [HttpGet("{accountHolderId}/active-courses")]
        public async Task<IActionResult> GetActiveCourses([FromRoute] string accountHolderId)
        {
            try
            {
                var res = await _enrollmentService.GetActiveCoursesForAccountAsync(accountHolderId);
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
