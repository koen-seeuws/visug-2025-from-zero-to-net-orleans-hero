using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using TheCodeKitchen.Application.Contracts.Events.Cook;
using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;
using TheCodeKitchen.Presentation.API.Cook.Models;

const string apiUrl = "https://ca-tck-cook-api.proudbeach-fbb36fdd.westeurope.azurecontainerapps.io/";
const string kitchenCode = "ZO7S";
const string username = "KOEN9";
const string password = "TEST";

var apiClient = new HttpClient { BaseAddress = new Uri(apiUrl) };

//Auth
var authRequest = new AuthenticationRequest(username, password);
var httpResponse = await apiClient.PostAsJsonAsync($"kitchen/{kitchenCode}/join", authRequest);
httpResponse.EnsureSuccessStatusCode();
var response = await httpResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
ArgumentNullException.ThrowIfNull(response);
ArgumentException.ThrowIfNullOrWhiteSpace(response.Token);

apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);

//SignalR
var cookConnection = new HubConnectionBuilder()
    .WithUrl($"{apiUrl}cookhub", options =>
    {
        options.AccessTokenProvider = () => Task.FromResult(response.Token)!;
    })
    .WithAutomaticReconnect()
    .Build();

cookConnection.On<KitchenOrderCreatedEvent>(nameof(KitchenOrderCreatedEvent), kitchenOrderEvent =>
{
    Console.WriteLine($"New Kitchen Order: {kitchenOrderEvent.Number} ");
});

cookConnection.On<TimerElapsedEvent>(nameof(TimerElapsedEvent), timerElapsedEvent =>
{
    Console.WriteLine($"Timer elapsed: {timerElapsedEvent.Number} - {timerElapsedEvent.Note}");
});

cookConnection.On<MessageReceivedEvent>(nameof(MessageReceivedEvent), messageReceivedEvent =>
{
    Console.WriteLine($"Message received: {messageReceivedEvent.Number} - {messageReceivedEvent.From} - {messageReceivedEvent.Content}");
});

await cookConnection.StartAsync();

Console.ReadLine();