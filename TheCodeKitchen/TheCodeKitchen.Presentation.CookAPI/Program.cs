using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Infrastructure.AzureSignalR;
using TheCodeKitchen.Infrastructure.Logging.Serilog;
using TheCodeKitchen.Infrastructure.Orleans.Client;
using TheCodeKitchen.Infrastructure.Security.Configuration;
using TheCodeKitchen.Presentation;
using TheCodeKitchen.Presentation.API.Cook.Hubs;
using TheCodeKitchen.Presentation.API.Cook.Validators;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.RegisterSerilog();

// Application services
builder.Services.AddValidationServices();
builder.Services.AddValidatorsFromAssembly(typeof(AuthenticationRequestValidator).Assembly);

// Infrastructure services
builder.Services.AddTheCodeKitchenOrleansClient(builder.Configuration);
builder.Services.AddJwtSecurityServices(builder.Configuration);
builder.Services.AddPasswordHashingServices();
builder.Services.AddAzureSignalRServices(builder.Configuration);
builder.Services.AddMemoryCache();

// Presentation services
builder.Services.AddHealthChecks().AddCheck<OrleansConnectionHealthCheck>(
    "orleans_connection_health_check",
    tags: ["orleans"]
);
builder.Services.AddHttpLogging();
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(swagger =>
{
    // Add Bearer token support
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("orleans")
});

app.MapControllers();

app.MapHub<CookHub>("/CookHub");

app.MapOpenApi();

app.UseHttpsRedirection();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();