using Microsoft.Extensions.DependencyInjection;
using WebhookEngine.Core.Abstractions;
using WebhookEngine.Core.Delivery;

namespace WebhookEngine.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddWebhookEngineCore(this IServiceCollection services)
    {
        services.AddScoped<IWebhookClient, WebhookClient>();
        return services;
    }
}
