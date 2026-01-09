using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.DTOs;
using MOE_System.Application.Interfaces;

namespace MOE_System.API.Controllers;

[ApiController]
[Route("api/education-accounts")]
public class EducationAccountsController : ControllerBase
{
    private readonly IEducationAccountService _educationAccountService;
    private readonly ILogger<EducationAccountsController> _logger;

    public EducationAccountsController(
        IEducationAccountService educationAccountService,
        ILogger<EducationAccountsController> logger)
    {
        _educationAccountService = educationAccountService;
        _logger = logger;
    }

    /// <summary>
    /// Get education account balance
    /// </summary>
    /// <param name="accountId">Education account ID</param>
    /// <returns>Account balance information</returns>
    [HttpGet("{accountId}/balance")]
    [ProducesResponseType(typeof(AccountBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountBalanceDto>> GetAccountBalance(string accountId)
    {
        try
        {
            var result = await _educationAccountService.GetAccountBalanceAsync(accountId);

            if (result == null)
            {
                return NotFound(new { message = $"Education account with ID {accountId} not found." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account balance for account {AccountId}", accountId);
            throw;
        }
    }

    /// <summary>
    /// Get education account outstanding fees
    /// </summary>
    /// <param name="accountId">Education account ID</param>
    /// <returns>Outstanding fee information</returns>
    [HttpGet("{accountId}/outstanding-fees")]
    [ProducesResponseType(typeof(OutstandingFeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OutstandingFeeDto>> GetOutstandingFees(string accountId)
    {
        try
        {
            var result = await _educationAccountService.GetOutstandingFeesAsync(accountId);

            if (result == null)
            {
                return NotFound(new { message = $"Education account with ID {accountId} not found." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving outstanding fees for account {AccountId}", accountId);
            throw;
        }
    }
}
