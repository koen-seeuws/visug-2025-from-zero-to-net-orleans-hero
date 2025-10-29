using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Events.Kitchen;
using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Presentation.ManagementUI.Models.ViewModels;
using TheCodeKitchen.Presentation.ManagementUI.Services;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Shared;

public partial class KitchenComponent(
    NavigationManager navigationManager,
    ISnackbar snackbar,
    IClusterClient clusterClient,
    IMapper mapper,
    ClientTimeService clientTimeService,
    ScrollService scrollService
) : ComponentBase, IAsyncDisposable
{
    private HubConnection? _kitchenHubConnection;
    private HubConnection? _kitchenOrderHubConnection;
    [Parameter] public KitchenViewModel Kitchen { get; set; } = null!;

    private ICollection<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
    private string? KitchenConnectionErrorMessage { get; set; }

    private ICollection<KitchenOrderViewModel>? KitchenOrders { get; set; }
    private string? KitchenOrdersErrorMessage { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_kitchenHubConnection is not null) await _kitchenHubConnection.DisposeAsync();
        if (_kitchenOrderHubConnection is not null) await _kitchenOrderHubConnection.DisposeAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadKitchenOrders();
        await ListenToKitchenEvents();
        await ListenToKitchenOrderEvents();
    }

    private async Task LoadKitchenOrders()
    {
        try
        {
            KitchenOrders = null;
            var kitchenGrain = clusterClient.GetGrain<IKitchenGrain>(Kitchen.Id);
            var getOpenOrdersResult = await kitchenGrain.GetOpenOrders();
            if (getOpenOrdersResult.Succeeded)
                KitchenOrders = mapper.Map<List<KitchenOrderViewModel>>(getOpenOrdersResult.Value);
            else
                KitchenOrdersErrorMessage = getOpenOrdersResult.Error.Message;
        }
        catch
        {
            KitchenOrdersErrorMessage = "An error occurred while retrieving the kitchen's orders.";
        }
    }

    private async Task ListenToKitchenEvents()
    {
        if (_kitchenHubConnection is not null)
        {
            await _kitchenHubConnection.DisposeAsync();
            _kitchenHubConnection = null;
        }

        _kitchenHubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri($"/KitchenHub?kitchenId={Kitchen.Id}"))
            .Build();

        _kitchenHubConnection.On(nameof(KitchenRatingUpdatedEvent), async (KitchenRatingUpdatedEvent @event) =>
        {
            Kitchen.Rating = @event.Rating;
            await InvokeAsync(StateHasChanged);
        });

        _kitchenHubConnection.On(nameof(MessageDeliveredEvent), async (MessageDeliveredEvent @event) =>
        {
            var messageViewModel = mapper.Map<MessageViewModel>(@event);
            Messages.Add(messageViewModel);
            await InvokeAsync(async () =>
            {
                StateHasChanged();
                await scrollService.ScrollToBottomIfPreviouslyNearBottom($"kitchen-{Kitchen.Id}-messages");
            });
        });
        
        _kitchenHubConnection.On(nameof(KitchenResetEvent), async (KitchenResetEvent _) =>
        {
            KitchenOrders?.Clear();
            Messages.Clear();
            await InvokeAsync(StateHasChanged);
        });

        try
        {
            await _kitchenHubConnection.StartAsync();
        }
        catch
        {
            KitchenConnectionErrorMessage = "Failed to start listening to kitchen events";
        }
    }

    private async Task ListenToKitchenOrderEvents()
    {
        if (_kitchenOrderHubConnection is not null)
        {
            await _kitchenOrderHubConnection.DisposeAsync();
            _kitchenOrderHubConnection = null;
        }

        _kitchenOrderHubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri($"/KitchenOrderHub?kitchenId={Kitchen.Id}"))
            .Build();

        _kitchenOrderHubConnection.On(nameof(KitchenOrderCreatedEvent), async (KitchenOrderCreatedEvent @event) =>
        {
            var kitchenOrderViewModel = mapper.Map<KitchenOrderViewModel>(@event);
            KitchenOrders = KitchenOrders?
                .Append(kitchenOrderViewModel)
                .OrderBy(o => o.Number)
                .ToList();
            
            await InvokeAsync(StateHasChanged);
        });

        _kitchenOrderHubConnection.On(nameof(KitchenOrderFoodDeliveredEvent), async (KitchenOrderFoodDeliveredEvent @event) =>
        {
            var kitchenOrder = KitchenOrders?.FirstOrDefault(o => o.Number == @event.Number);
            if (kitchenOrder is not null)
            {
                kitchenOrder.DeliveredFoods = kitchenOrder.DeliveredFoods
                    .Append(@event.Food)
                    .Order()
                    .ToList();
            }

            await InvokeAsync(StateHasChanged);
        });

        _kitchenOrderHubConnection.On(nameof(KitchenOrderCompletedEvent), async (KitchenOrderCompletedEvent @event) =>
        {
            var kitchenOrder = KitchenOrders?.FirstOrDefault(o => o.Number == @event.Number);
            if (kitchenOrder is not null)
                KitchenOrders?.Remove(kitchenOrder);
            await InvokeAsync(StateHasChanged);
        });

        try
        {
            await _kitchenOrderHubConnection.StartAsync();
        }
        catch
        {
            snackbar.Add("Failed to start listening to kitchen events", Severity.Error);
        }
    }
}