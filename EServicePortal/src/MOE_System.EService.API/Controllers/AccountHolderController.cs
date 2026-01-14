using Microsoft.AspNetCore.Mvc;
using MOE_System.EService.Application.Common;
using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;

namespace MOE_System.EService.API.Controllers
{

    [Route("api/v1/account-holders")]
    public class AccountHolderController : BaseApiController
    {
        private readonly IAccountHolderService _accountHolderService;
        private readonly IEnrollmentService _enrollmentService;

        public AccountHolderController(IAccountHolderService accountHolderService, IEnrollmentService enrollmentService)
        {
            _accountHolderService = accountHolderService;
            _enrollmentService = enrollmentService;
        }

        [HttpGet("{accountHolderId}")]
        public async Task<ActionResult<ApiResponse<AccountHolderResponse>>> GetAccountHolder([FromRoute] string accountHolderId)
        {
            var accountHolderResponse = await _accountHolderService.GetAccountHolderAsync(accountHolderId);

            return Success(accountHolderResponse);
        }

        [HttpGet("{accountHolderId}/active-courses")]
        public async Task<ActionResult<ApiResponse<PaginatedList<ActiveCoursesResponse>>>> GetActiveCourses([FromRoute] string accountHolderId, 
            [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResultResponse = await _enrollmentService.GetActiveCoursesAsync(accountHolderId, pageIndex, pageSize);

            return Paginated(pagedResultResponse.Items, pagedResultResponse.PageIndex, pagedResultResponse.PageSize, pagedResultResponse.TotalCount);
        }
    }
}
