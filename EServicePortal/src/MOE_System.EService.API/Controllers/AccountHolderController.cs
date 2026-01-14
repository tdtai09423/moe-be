using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOE_System.EService.Application.Common;
using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using System.Security.Claims;

namespace MOE_System.EService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/account-holders")]
public class AccountHolderController : ControllerBase
{
    private readonly IAccountHolderService _accountHolderService;
    private readonly IEnrollmentService _enrollmentService;

    public AccountHolderController(IAccountHolderService accountHolderService, IEnrollmentService enrollmentService)
    {
        _accountHolderService = accountHolderService;
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Get current authenticated account holder's profile
    /// </summary>
    /// <returns>Account holder profile information</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(AccountHolderProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountHolderProfileResponse>> GetMyProfile()
    {
        // Get the account holder ID from JWT token claims
        var accountHolderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(accountHolderId))
        {
            return Unauthorized("Invalid or missing authentication token");
        }

        var profile = await _accountHolderService.GetMyProfileAsync(accountHolderId);
        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UpdateProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var accountHolderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(accountHolderId))
        {
            return Unauthorized("Invalid or missing authentication token");
        }
        var response = await _accountHolderService.UpdateProfileAsync(accountHolderId, request);
        return Ok(response);
    }

    [HttpGet("{accountHolderId}")]
    public async Task<ActionResult<AccountHolderResponse>> GetAccountHolder([FromRoute] string accountHolderId)
    {
        var accountHolderResponse = await _accountHolderService.GetAccountHolderAsync(accountHolderId);
        return Ok(accountHolderResponse);
    }

    [HttpGet("{accountHolderId}/active-courses")]
    public async Task<ActionResult<PaginatedList<ActiveCoursesResponse>>> GetActiveCourses([FromRoute] string accountHolderId, 
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var pagedResultResponse = await _enrollmentService.GetActiveCoursesAsync(accountHolderId, pageIndex, pageSize);
        return Ok(pagedResultResponse);
    }
}
