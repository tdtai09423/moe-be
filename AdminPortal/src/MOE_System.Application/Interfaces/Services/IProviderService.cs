using MOE_System.Application.DTOs.Provider.Response;

namespace MOE_System.Application.Interfaces.Services;

public interface IProviderService
{
    Task<IReadOnlyList<ProviderListResponse>> GetAllProvidersAsync(CancellationToken cancellationToken);
}