using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Events.Kitchen;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Response.Game;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;
using TheCodeKitchen.Presentation.ManagementUI.Components.Dialogs;
using TheCodeKitchen.Presentation.ManagementUI.Models.TableRecordModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Pages;

public partial class PreGameLobby(
    NavigationManager navigationManager,
    IDialogService dialogService,
    ISnackbar snackbar,
    IClusterClient clusterClient,
    IMapper mapper
) : ComponentBase, IAsyncDisposable
{
    private HubConnection? _gameHubConnection;
    [Parameter] public Guid GameId { get; set; }
    private GetGameResponse? GetGameResponse { get; set; }
    private ICollection<KitchenTableRecordModel>? KitchenRecords { get; set; }
    private IDictionary<Guid, List<CookTableRecordModel>>? CookRecordsPerKitchen { get; set; }
    private string? ErrorMessage { get; set; }
    public bool Busy { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_gameHubConnection is not null) await _gameHubConnection.DisposeAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadGame();
        await LoadKitchens();
        await LoadCooks();
        await ListenToGameEvents();
        await base.OnInitializedAsync();
    }

    private async Task LoadGame()
    {
        try
        {
            GetGameResponse = null;
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var getGameResult = await gameGrain.GetGame();
            if (getGameResult.Succeeded)
                GetGameResponse = getGameResult.Value;
            else
                ErrorMessage = getGameResult.Error.Message;
        }
        catch
        {
            ErrorMessage = "An error occurred while retrieving the game.";
        }
    }

    private async Task LoadKitchens()
    {
        try
        {
            KitchenRecords = null;
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var getKitchensResult = await gameGrain.GetKitchens();
            if (getKitchensResult.Succeeded)
            {
                KitchenRecords = mapper.Map<List<KitchenTableRecordModel>>(getKitchensResult.Value);
            }
            else
                ErrorMessage = getKitchensResult.Error.Message;
        }
        catch
        {
            ErrorMessage = "An error occurred while retrieving the kitchens.";
        }
    }

    private async Task LoadCooks()
    {
        try
        {
            CookRecordsPerKitchen = null;

            if (KitchenRecords is null)
            {
                throw new NullReferenceException();
            }

            var getCookRequest = new GetCookRequest(null);

            var cooksPerKitchen = new Dictionary<Guid, List<CookTableRecordModel>>();

            foreach (var kitchen in KitchenRecords)
            {
                var kitchenGrain = clusterClient.GetGrain<IKitchenGrain>(kitchen.Id);
                var getCooksResult = await kitchenGrain.GetCooks(getCookRequest);
                if (getCooksResult.Succeeded)
                {
                    var cookRecords = mapper.Map<List<CookTableRecordModel>>(getCooksResult.Value);
                    cooksPerKitchen[kitchen.Id] = cookRecords;
                }
                else
                {
                    ErrorMessage = getCooksResult.Error.Message;
                    break;
                }
            }

            CookRecordsPerKitchen = cooksPerKitchen;
        }
        catch
        {
            ErrorMessage = "An error occurred while retrieving the cooks.";
        }
    }


    private async Task ListenToGameEvents()
    {
        if (GetGameResponse?.Started is not null)
            return;

        if (_gameHubConnection is not null)
            await _gameHubConnection.DisposeAsync();

        _gameHubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri($"/GameHub?gameId={GameId}"))
            .Build();

        _gameHubConnection.On(nameof(KitchenCreatedEvent), async (KitchenCreatedEvent @event) =>
        {
            var kitchenRecord = mapper.Map<KitchenTableRecordModel>(@event);
            KitchenRecords?.Add(kitchenRecord);
            if (CookRecordsPerKitchen is not null)
                CookRecordsPerKitchen[kitchenRecord.Id] = [];
            await InvokeAsync(StateHasChanged);
        });

        _gameHubConnection.On(nameof(CookJoinedEvent), async (CookJoinedEvent @event) =>
        {
            if (CookRecordsPerKitchen is null) return;

            if (!CookRecordsPerKitchen.TryGetValue(@event.Kitchen, out var cookRecords))
            {
                cookRecords = [];
                CookRecordsPerKitchen[@event.Kitchen] = cookRecords;
            }

            var cookRecord = mapper.Map<CookTableRecordModel>(@event);
            cookRecords.Add(cookRecord);
            await InvokeAsync(StateHasChanged);
        });

        try
        {
            await _gameHubConnection.StartAsync();
        }
        catch
        {
            snackbar.Add("Failed to start listening to new kitchen events", Severity.Error);
        }
    }


    private async Task CreateKitchen()
    {
        var dialogParameters = new DialogParameters { { nameof(CreateKitchenDialog.GameId), GameId } };

        var dialog =
            await dialogService.ShowAsync<CreateKitchenDialog>("Create Kitchen", dialogParameters);
        var dialogResult = await dialog.Result;

        if (dialogResult is { Canceled: false, Data: CreateKitchenResponse createKitchenResponse })
        {
            snackbar.Add($"Successfully created kitchen {createKitchenResponse.Game}", Severity.Success);
        }
    }

    private async Task StartGame()
    {
        Busy = true;
        try
        {
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var startGameResult = await gameGrain.StartGame();
            if (startGameResult.Succeeded)
            {
                snackbar.Add("Game started successfully.", Severity.Success);
                navigationManager.NavigateTo($"/games/{GameId}");
            }
            else
            {
                snackbar.Add(startGameResult.Error.Message, Severity.Error);
            }
        }
        catch
        {
            snackbar.Add("An error occurred while starting the game.", Severity.Error);
        }
        finally
        {
            Busy = false;
        }
    }
}