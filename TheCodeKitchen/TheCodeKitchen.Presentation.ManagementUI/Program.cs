using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MudBlazor.Services;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Infrastructure.AzureSignalR;
using TheCodeKitchen.Infrastructure.Logging.Serilog;
using TheCodeKitchen.Infrastructure.Orleans.Client;
using TheCodeKitchen.Presentation;
using TheCodeKitchen.Presentation.ManagementUI.Components;
using TheCodeKitchen.Presentation.ManagementUI.Hubs;
using TheCodeKitchen.Presentation.ManagementUI.Mapping;
using TheCodeKitchen.Presentation.ManagementUI.Services;
using TheCodeKitchen.Presentation.ManagementUI.Validation;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.RegisterSerilog();

// Application services
builder.Services.AddAutoMapper(typeof(GameMapping));
builder.Services.AddValidationServices();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateRecipeFormModelValidator));

// Infrastructure services
builder.Services.AddTheCodeKitchenOrleansClient(builder.Configuration);
builder.Services.AddAzureSignalRServices(builder.Configuration);

// Presentation services
builder.Services.AddHealthChecks().AddCheck<OrleansConnectionHealthCheck>(
    "orleans_connection_health_check",
    tags: ["orleans"]
);
builder.Services.AddMudServices();
builder.Services.AddScoped<ClientTimeService>();
builder.Services.AddScoped<ScrollService>();

builder.Services.AddHttpLogging();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseHttpLogging();

app.UseAntiforgery();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("orleans")
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<GameManagementHub>("/GameManagementHub");
app.MapHub<GameHub>("/GameHub");
app.MapHub<KitchenHub>("/KitchenHub");
app.MapHub<KitchenOrderHub>("/KitchenOrderHub");

app.Run();