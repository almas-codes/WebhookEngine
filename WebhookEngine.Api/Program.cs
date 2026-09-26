using WebhookEngine.Core;
using WebhookEngine.Infrastructure;
using WebhookEngine.Api.Features.RegisterEndpoint;
using WebhookEngine.Api.Features.SendWebhook;
using WebhookEngine.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebhookEngineCore();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

RegisterEndpointFeature.MapEndpoint(app);
SendWebhookFeature.MapEndpoint(app);

app.Run();

public partial class Program { }
