using MOE_System.Application;
using MOE_System.Infrastructure;
using MOE_System.API.ServicesRegister;
using MOE_System.API.MiddleWares;
using MOE_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MOE_System.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(
    options => options.Filters.Add<ValidationFilter>()
);

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerConfiguration();

// Add CORS - Allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Auto-migrate database on startup (Development only)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            dbContext.Database.Migrate();
            app.Logger.LogInformation("Database migrated successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while migrating the database");
        }
    }
}

// Configure exception handling middleware
app.UseExceptionMiddleware();

// Configure the HTTP request pipeline.
app.UseSwaggerConfiguration(app.Environment);

// CORS must be before HTTPS redirection and authorization
app.UseCors("AllowSpecificOrigins");

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
