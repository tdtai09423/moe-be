using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOE_System.Domain.Entities;

namespace MOE_System.Infrastructure.Data.Seeding;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedProvidersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    public async Task SeedProvidersAsync()
    {
        if (await _context.Providers.AnyAsync())
        {
            _logger.LogInformation("Providers already seeded. Skipping...");
            return;
        }

        _logger.LogInformation("Seeding providers...");

        var providersData = ProviderSeedData.GetProviders();
        var providers = providersData.Select(kvp => new Provider
        {
            Id = kvp.Key,
            Name = kvp.Value
        }).ToList();

        await _context.Providers.AddRangeAsync(providers);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} providers successfully", providers.Count);
    }

    public async Task<object> GetSeedStatusAsync()
    {
        var providersCount = await _context.Providers.CountAsync();
        var providersSeeded = providersCount > 0;

        return new
        {
            providers = new
            {
                isSeeded = providersSeeded,
                count = providersCount,
                totalAvailable = ProviderSeedData.GetProviders().Count
            }
        };
    }
}
