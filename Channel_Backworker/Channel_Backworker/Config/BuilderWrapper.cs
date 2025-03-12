using System.Threading.Channels;
using Student_Coordinator;
using Student_Coordinator.Message;

namespace Channel_Backworker.Config;

public static class BuilderWrapper
{
    public static IServiceCollection AddSwagger(this IServiceCollection provider)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        provider.AddOpenApi(); // check if needed
        provider.AddEndpointsApiExplorer();
        provider.AddSwaggerGen();

        return provider;
    }

    public static IServiceCollection AddExternalServices(this IServiceCollection provider)
    {
        //var channel = Channel.CreateUnbounded<RequestBackgroundService>();
        provider.AddSingleton(Channel.CreateUnbounded<RequestBackgroundService>());
        provider.AddHostedService<Startup>();
        
        //provider.AddSingleton(channel);
        // do something here
        return provider;
    }
}

public static class WebAppWrapper
{
    public static IApplicationBuilder UseSwaggerConf(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Channel_Backworker v1");
            c.RoutePrefix = string.Empty;
        });
        return app;
    }
}