using Microsoft.AspNetCore.Mvc;
using MOE_System.Infrastructure.Data.Seeding;
using MOE_System.Infrastructure.Data;

namespace MOE_System.API.Controllers;

[ApiController]
[Route("api/seeder")]
public class SeedController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _seederLogger;
    private readonly ILogger<SeedController> _logger;

    public SeedController(
        ApplicationDbContext context,
        ILogger<DatabaseSeeder> seederLogger,
        ILogger<SeedController> logger)
    {
        _context = context;
        _seederLogger = seederLogger;
        _logger = logger;
    }

    [HttpPost("all")]
    public async Task<IActionResult> SeedAll()
    {
        try
        {
            var seeder = new DatabaseSeeder(_context, _seederLogger);
            await seeder.SeedAsync();
            
            return Ok(new { message = "Database seeded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            return StatusCode(500, new { message = "Error seeding database", error = ex.Message });
        }
    }

    [HttpPost("providers")]
    public async Task<IActionResult> SeedProviders()
    {
        try
        {
            var seeder = new DatabaseSeeder(_context, _seederLogger);
            await seeder.SeedProvidersAsync();
            
            return Ok(new { message = "Providers seeded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding providers");
            return StatusCode(500, new { message = "Error seeding providers", error = ex.Message });
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetSeedStatus()
    {
        try
        {
            var seeder = new DatabaseSeeder(_context, _seederLogger);
            var status = await seeder.GetSeedStatusAsync();
            
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seed status");
            return StatusCode(500, new { message = "Error getting seed status", error = ex.Message });
        }
    }
}
