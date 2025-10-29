using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Events.GameManagement;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Response.Game;
using TheCodeKitchen.Presentation.ManagementUI.Components.Dialogs;
using TheCodeKitchen.Presentation.ManagementUI.Models.TableRecordModels;
using TheCodeKitchen.Presentation.ManagementUI.Services;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Pages;

public partial class Games(
    NavigationManager navigationManager,
    IDialogService dialogService,
    ISnackbar snackbar,
    IClusterClient clusterClient,
    IMapper mapper,
    ClientTimeService clientTimeService
) : ComponentBase, IAsyncDisposable
{
    private HubConnection? _gameManagementHubConnection;
    private ICollection<GameTableRecordModel>? GameRecords { get; set; }
    private string? ErrorMessage { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_gameManagementHubConnection is not null) await _gameManagementHubConnection.DisposeAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadGames();
        await ListenToGameCreatedEvents();
        await base.OnInitializedAsync();
    }

    private async Task LoadGames()
    {
        try
        {
            GameRecords = null;
            var gameManagementGrain = clusterClient.GetGrain<IGameManagementGrain>(Guid.Empty);
            var result = await gameManagementGrain.GetGames();
            if (result.Succeeded)
                GameRecords = mapper.Map<List<GameTableRecordModel>>(result.Value);
            else
                ErrorMessage = result.Error.Message;
        }
        catch
        {
            ErrorMessage = "An error occurred while retrieving the games.";
        }
    }

    private async Task ListenToGameCreatedEvents()
    {
        if (_gameManagementHubConnection is not null)
            await _gameManagementHubConnection.DisposeAsync();

        _gameManagementHubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/GameManagementHub"))
            .Build();

        _gameManagementHubConnection.On(nameof(GameCreatedEvent), async (GameCreatedEvent @event) =>
        {
            var gameRecord = mapper.Map<GameTableRecordModel>(@event);
            GameRecords?.Add(gameRecord);
            await InvokeAsync(StateHasChanged);
        });

        try
        {
            await _gameManagementHubConnection.StartAsync();
        }
        catch
        {
            snackbar.Add("Failed to start listening to new game events", Severity.Error);
        }
    }

    private void NavigateToGame(Guid gameId, bool started)
    {
        var link = $"/games/{gameId}";
        if (!started)
            link += "/pre-game-lobby";
        navigationManager.NavigateTo(link);
    }

    private async Task CreateGame()
    {
        var dialog = await dialogService.ShowAsync<CreateGameDialog>("Create Game");
        var dialogResult = await dialog.Result;

        if (dialogResult is { Canceled: false, Data: CreateGameResponse createGameResponse })
            NavigateToGame(createGameResponse.Id, false);
    }
}