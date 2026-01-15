using MOE_System.EService.Application;
using MOE_System.EService.Infrastructure;
using MOE_System.EService.API.ServicesRegister;
using MOE_System.EService.API.MiddleWares;
using MOE_System.EService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MOE_System.EService.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(
    options => options.Filters.Add<ValidationFilter>()
);

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerConfiguration();

// Add CORS
var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            if (allowedOrigins.Contains("*"))
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
        });
});

var app = builder.Build();

// Database First approach - no auto-migration needed
// DbContext and entities are scaffolded from existing database

// Configure exception handling middleware
app.UseExceptionMiddleware();

// Configure the HTTP request pipeline.
app.UseSwaggerConfiguration(app.Environment);

app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
