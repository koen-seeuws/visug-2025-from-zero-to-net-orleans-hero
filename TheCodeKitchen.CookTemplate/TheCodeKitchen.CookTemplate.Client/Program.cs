using TheCodeKitchen.CookTemplate.Client;

const string apiUrl = "https://ca-tck-cook-api.proudbeach-fbb36fdd.westeurope.azurecontainerapps.io/";


var theCodeKitchenClient = new TheCodeKitchenClient(apiUrl);
var cook = new Cook(theCodeKitchenClient);// <- Implement your code in the Cook class

var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // Prevent immediate termination
    cancellationTokenSource.Cancel();
    Console.WriteLine("\nStopping cooking...");
};

try
{
    Console.WriteLine("Starting cooking...");
    await cook.StartCooking(cancellationTokenSource.Token);
    Console.WriteLine("\nStarted cooking. Press Ctrl+C to stop.");
    await Task.Delay(-1, cancellationTokenSource.Token); // Keep the app alive until Ctrl+C
}
catch (TaskCanceledException)
{
    Console.WriteLine("\nStopped cooking.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
