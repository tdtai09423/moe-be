using Microsoft.OpenApi;

namespace MOE_System.EService.API.ServicesRegister;

public static class SwaggerServiceRegistration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Input your Json Web Token here"
            });

            options.AddSecurityRequirement(documents =>
            {
                var requirement = new OpenApiSecurityRequirement();
                var scheme = documents.Components?.SecuritySchemes?["Bearer"];

                if(scheme != null)
                {
                    requirement[new OpenApiSecuritySchemeReference("Bearer", documents)] = new List<string>();
                }

                return requirement;
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MOE System API v1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "MOE System API Documentation";
            c.DisplayRequestDuration();
        });

        return app;
    }
}
