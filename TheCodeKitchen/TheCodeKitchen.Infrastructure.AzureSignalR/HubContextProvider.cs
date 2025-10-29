using System.Collections.Concurrent;
using Microsoft.Azure.SignalR.Management;

namespace TheCodeKitchen.Infrastructure.AzureSignalR;

public sealed class HubContextProvider(ServiceManager serviceManager) : IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, Lazy<Task<ServiceHubContext>>> _contexts = new(StringComparer.OrdinalIgnoreCase);
    private readonly ServiceManager _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));

    public async ValueTask DisposeAsync()
    {
        foreach (var lazy in _contexts.Values)
        {
            if (!lazy.IsValueCreated) continue;
            var context = await lazy.Value;
            await context.DisposeAsync();
        }
    }

    public Task<ServiceHubContext> GetHubContextAsync(string hubName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(hubName)) throw new ArgumentNullException(nameof(hubName));

        // Use Lazy<Task<>> to ensure single initialization per hub name
        var lazyHubContext = _contexts.GetOrAdd(hubName, name =>
            new Lazy<Task<ServiceHubContext>>(() => 
                _serviceManager.CreateHubContextAsync(name, cancellationToken: cancellationToken)));

        // Return the Task<IServiceHubContext>
        return lazyHubContext.Value;
    }
}