using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using TheCodeKitchen.CookTemplate.Contracts.Events.Communication;
using TheCodeKitchen.CookTemplate.Contracts.Events.Order;
using TheCodeKitchen.CookTemplate.Contracts.Events.Timer;
using TheCodeKitchen.CookTemplate.Contracts.Reponses.Authentication;
using TheCodeKitchen.CookTemplate.Contracts.Reponses.Communication;
using TheCodeKitchen.CookTemplate.Contracts.Reponses.CookBook;
using TheCodeKitchen.CookTemplate.Contracts.Reponses.Food;
using TheCodeKitchen.CookTemplate.Contracts.Reponses.Orders;
using TheCodeKitchen.CookTemplate.Contracts.Reponses.Pantry;
using TheCodeKitchen.CookTemplate.Contracts.Reponses.Timer;
using TheCodeKitchen.CookTemplate.Contracts.Requests.Authentication;
using TheCodeKitchen.CookTemplate.Contracts.Requests.Communication;
using TheCodeKitchen.CookTemplate.Contracts.Requests.Timer;

namespace TheCodeKitchen.CookTemplate.Client;

public class TheCodeKitchenClient
{
    private readonly Uri _baseUrl;
    private readonly HttpClient _httpClient;
    private string? _token;
    private HubConnection? _cookHubConnection;

    public TheCodeKitchenClient(string baseUrl)
    {
        if (!baseUrl.EndsWith('/'))
            baseUrl += '/';

        _baseUrl = new Uri(baseUrl);
        _httpClient = new HttpClient { BaseAddress = _baseUrl };
    }

    public async Task Connect(
        string username,
        string password,
        string kitchenCode,
        Action<KitchenOrderCreatedEvent> onKitchenOrderCreated,
        Action<TimerElapsedEvent> onTimerElapsed,
        Action<MessageReceivedEvent> onMessageReceived,
        CancellationToken cancellationToken = default
    )
    {
        await Authenticate(username, password, kitchenCode, cancellationToken);
        await ListenToCookHubEvents(onKitchenOrderCreated, onTimerElapsed, onMessageReceived, cancellationToken);
    }

    // Communication
    public async Task SendMessage(SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("Communication/SendMessage", request,
            cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ReadMessageResponse[]> ReadMessages(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("Communication/ReadMessages", cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        return
            await response.Content.ReadFromJsonAsync<ReadMessageResponse[]>(cancellationToken: cancellationToken) ??
            [];
    }

    public async Task ConfirmMessage(ConfirmMessageRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("Communication/ConfirmMessage", request,
            cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    // CookBook
    public async Task<GetRecipeResponse[]> ReadRecipes(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("CookBook/Read", cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        return
            await response.Content.ReadFromJsonAsync<GetRecipeResponse[]>(cancellationToken: cancellationToken) ??
            [];
    }

    // Equipment
    public async Task AddFoodToEquipment(string equipment, int number, CancellationToken cancellationToken = default)
    {
        equipment = equipment.Replace(" ", string.Empty);
        var response = await _httpClient.PostAsync($"Equipment/{equipment}/{number}/AddFood", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TakeFoodResponse?> TakeFoodFromEquipment(string equipment, int number,
        CancellationToken cancellationToken = default)
    {
        equipment = equipment.Replace(" ", string.Empty);
        var response = await _httpClient.PostAsync($"Equipment/{equipment}/{number}/TakeFood", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TakeFoodResponse>(cancellationToken: cancellationToken) ?? null;
    }

    // Orders
    public async Task<GetOpenOrderResponse[]> ViewOpenOrders(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("Orders/ViewOpen", cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        return
            await response.Content.ReadFromJsonAsync<GetOpenOrderResponse[]>(cancellationToken: cancellationToken) ??
            [];
    }

    public async Task DeliverFoodToOrder(long orderNumber, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"Orders/{orderNumber}/Deliver", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task CompleteOrder(long orderNumber, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"Orders/{orderNumber}/Complete", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    // Pantry
    public async Task<GetIngredientResponse[]> PantryInventory(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("Pantry/Inventory",
            cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        return
            await response.Content.ReadFromJsonAsync<GetIngredientResponse[]>(cancellationToken: cancellationToken) ??
            [];
    }

    public async Task<TakeFoodResponse?> TakeFoodFromPantry(string ingredient,
        CancellationToken cancellationToken = default)
    {
        ingredient = Uri.EscapeDataString(ingredient);
        var response = await _httpClient.PostAsync($"Pantry/{ingredient}/TakeFood", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TakeFoodResponse>(cancellationToken: cancellationToken) ?? null;
    }

    // Timer
    public async Task SetTimer(SetTimerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("Timer/Set", request, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetTimerResponse[]> GetTimers(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("Timer/Get", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetTimerResponse[]>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task StopTimer(StopTimerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("Timer/Stop", request, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    // Trash
    public async Task ThrowFoodAway(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("Trash/ThrowAway", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    // Kitchen (/Authentication)
    private async Task Authenticate(string username, string password, string kitchenCode,
        CancellationToken cancellationToken = default)
    {
        var authenticationRequest = new AuthenticationRequestDto(username, password);
        var httpResponse = await _httpClient.PostAsJsonAsync($"kitchen/{kitchenCode}/join", authenticationRequest,
            cancellationToken: cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        var authenticationResponse =
            await httpResponse.Content.ReadFromJsonAsync<AuthenticationResponseDto>(
                cancellationToken: cancellationToken);

        ArgumentNullException.ThrowIfNull(authenticationResponse);
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationResponse.Token);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _token = authenticationResponse.Token);
    }

    private async Task ListenToCookHubEvents(
        Action<KitchenOrderCreatedEvent> onKitchenOrderCreated,
        Action<TimerElapsedEvent> onTimerElapsed,
        Action<MessageReceivedEvent> onMessageReceived,
        CancellationToken cancellationToken = default
    )
    {
        _cookHubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri(_baseUrl, "CookHub"),
                options => { options.AccessTokenProvider = () => Task.FromResult(_token); })
            .WithAutomaticReconnect()
            .Build();

        _cookHubConnection.On(nameof(KitchenOrderCreatedEvent), onKitchenOrderCreated);
        _cookHubConnection.On(nameof(TimerElapsedEvent), onTimerElapsed);
        _cookHubConnection.On(nameof(MessageReceivedEvent), onMessageReceived);

        await _cookHubConnection.StartAsync(cancellationToken);
    }
}