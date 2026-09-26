using WebhookEngine.Worker;
using WebhookEngine.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("WebhookClient");
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<DeliveryWorker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

host.Run();
