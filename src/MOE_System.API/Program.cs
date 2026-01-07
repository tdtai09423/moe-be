using MOE_System.Application;
using MOE_System.Infrastructure;
using MOE_System.API.ServicesRegister;
using MOE_System.API.MiddleWares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Configure exception handling middleware
app.UseExceptionMiddleware();

// Configure the HTTP request pipeline.
app.UseSwaggerConfiguration(app.Environment);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
