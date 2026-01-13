namespace MOE_System.API.ServicesRegister;

public static class SwaggerServiceRegistration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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
