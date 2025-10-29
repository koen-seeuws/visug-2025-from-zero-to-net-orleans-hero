using TheCodeKitchen.Infrastructure.Logging.Serilog;
using TheCodeKitchen.Infrastructure.Orleans.Client;
using TheCodeKitchen.Infrastructure.Orleans.Scaler.Services;
using TheCodeKitchen.Infrastructure.Orleans.Scaler.Validation;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.RegisterSerilog();

// Application services
builder.Services.AddSingleton<ScaledObjectRefValidator>();

// Infrastructure services
builder.Services.AddTheCodeKitchenOrleansClient(builder.Configuration);

// Presentation services
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ExternalScalerService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();