using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebhookEngine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            
        services.AddScoped<WebhookEngine.Core.Abstractions.IWebhookRepository, WebhookEngine.Infrastructure.Repositories.WebhookRepository>();
        return services;
    }
}
