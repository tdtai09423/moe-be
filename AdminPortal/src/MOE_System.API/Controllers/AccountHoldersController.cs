using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.DTOs;
using MOE_System.Application.Interfaces;

namespace MOE_System.API.Controllers;

[ApiController]
[Route("api/account-holders")]
public class AccountHoldersController : ControllerBase
{
    private readonly IAccountHolderService _accountHolderService;
    private readonly ILogger<AccountHoldersController> _logger;

    public AccountHoldersController(
        IAccountHolderService accountHolderService,
        ILogger<AccountHoldersController> logger)
    {
        _accountHolderService = accountHolderService;
        _logger = logger;
    }

    /// <summary>
    /// Get account holder basic information
    /// </summary>
    /// <param name="accountHolderId">Account holder ID</param>
    /// <returns>Account holder information</returns>
    [HttpGet("{accountHolderId}")]
    [ProducesResponseType(typeof(AccountHolderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountHolderDto>> GetAccountHolder(string accountHolderId)
    {
        try
        {
            var result = await _accountHolderService.GetAccountHolderByIdAsync(accountHolderId);

            if (result == null)
            {
                return NotFound(new { message = $"Account holder with ID {accountHolderId} not found." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account holder {AccountHolderId}", accountHolderId);
            throw;
        }
    }

    /// <summary>
    /// Get account holder's active courses
    /// </summary>
    /// <param name="accountHolderId">Account holder ID</param>
    /// <returns>List of active courses</returns>
    [HttpGet("{accountHolderId}/active-courses")]
    [ProducesResponseType(typeof(List<ActiveCourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ActiveCourseDto>>> GetActiveCourses(string accountHolderId)
    {
        try
        {
            var result = await _accountHolderService.GetActiveCoursesAsync(accountHolderId);

            // Return empty list if account holder has no active courses
            // This is valid behavior, so we return 200 OK instead of 404
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active courses for account holder {AccountHolderId}", accountHolderId);
            throw;
        }
    }
}
