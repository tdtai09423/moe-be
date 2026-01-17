using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.Interfaces.Services;

namespace MOE_System.API.Controllers;

[ApiController]
[Route("api/v1/admin/providers")]
public class ProviderController : ControllerBase
{
    private readonly IProviderService _providerService;

    public ProviderController(IProviderService providerService)
    {
        _providerService = providerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProviders(CancellationToken cancellationToken)
    {
        var providers = await _providerService.GetAllProvidersAsync(cancellationToken);
        return Ok(providers);
    }
}
