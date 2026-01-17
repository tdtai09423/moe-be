using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.Interfaces.Services;

namespace MOE_System.API.Controllers;

[ApiController]
[Route("api/v1/admin/education-accounts")]
public class EducationAccountController : ControllerBase
{
    private readonly IEducationAccountService _educationAccountService;

    public EducationAccountController(IEducationAccountService educationAccountService)
    {
        _educationAccountService = educationAccountService;
    }

    [HttpDelete("close/{nric}")]
    public async Task<IActionResult> CloseEducationAccounts([FromRoute] string nric, CancellationToken cancellationToken)
    {
        await _educationAccountService.CloseEducationAccountManuallyAsync(nric, cancellationToken);
        return Ok(new { Message = "Education accounts closure process initiated successfully." });
    }
}