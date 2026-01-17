using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs.Provider.Response;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Domain.Entities;

namespace MOE_System.Application.Services;

public class ProviderService : IProviderService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProviderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ProviderListResponse>> GetAllProvidersAsync(CancellationToken cancellationToken)
    {
        var providerRepository = _unitOfWork.GetRepository<Provider>();

        var providers = await providerRepository.ToListAsync(cancellationToken: cancellationToken);

        return providers.Select(p => new ProviderListResponse(
            p.Id,
            p.Name
        )).ToList();
    }
}