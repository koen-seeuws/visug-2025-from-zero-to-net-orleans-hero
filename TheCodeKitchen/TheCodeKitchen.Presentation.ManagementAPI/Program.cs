using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Infrastructure.AzureSignalR;
using TheCodeKitchen.Infrastructure.Logging.Serilog;
using TheCodeKitchen.Infrastructure.Orleans.Client;
using TheCodeKitchen.Infrastructure.Security.Configuration;
using TheCodeKitchen.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.RegisterSerilog();

// Application services
builder.Services.AddValidationServices();

// Infrastructure services
builder.Services.AddTheCodeKitchenOrleansClient(builder.Configuration);
builder.Services.AddPasswordHashingServices();
builder.Services.AddJwtSecurityServices(builder.Configuration);
builder.Services.AddAzureSignalRServices(builder.Configuration);

// Presentation services
builder.Services.AddHealthChecks().AddCheck<OrleansConnectionHealthCheck>(
    "orleans_connection_health_check",
    tags: ["orleans"]
);
builder.Services.AddHttpLogging();
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("orleans")
});

app.MapControllers();

app.MapOpenApi();

app.UseHttpsRedirection();

app.UseHttpLogging();

// app.UseAuthentication();
// app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();