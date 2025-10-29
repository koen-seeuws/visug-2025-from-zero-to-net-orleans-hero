using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Response.Game;
using TheCodeKitchen.Presentation.ManagementUI.Components.Dialogs;
using TheCodeKitchen.Presentation.ManagementUI.Models.ViewModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Pages;

public partial class Game(
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
    private string? GameErrorMessage { get; set; }
    private ICollection<KitchenViewModel>? Kitchens { get; set; }
    private string? KitchenErrorMessage { get; set; }
    private bool? Paused { get; set; }
    private TimeSpan TimePassed { get; set; }
    public bool Busy { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_gameHubConnection is not null) await _gameHubConnection.DisposeAsync();
    }


    protected override async Task OnInitializedAsync()
    {
        await LoadGame();
        await LoadKitchens();
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
            {
                GetGameResponse = getGameResult.Value;
                Paused = getGameResult.Value.Paused;
                TimePassed = getGameResult.Value.TimePassed;
            }
            else
                GameErrorMessage = getGameResult.Error.Message;
        }
        catch
        {
            GameErrorMessage = "An error occurred while retrieving the game.";
        }
    }

    private async Task LoadKitchens()
    {
        try
        {
            Kitchens = null;
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var getKitchensResult = await gameGrain.GetKitchens();
            if (getKitchensResult.Succeeded)
                Kitchens = getKitchensResult.Value
                    .Select(k => new KitchenViewModel
                    {
                        Id = k.Id,
                        Name = k.Name,
                        Code = k.Code,
                        Rating = k.OrderRatings.Values.DefaultIfEmpty(1.0).Average()
                    })
                    .ToList();
            else
                KitchenErrorMessage = getKitchensResult.Error.Message;
        }
        catch
        {
            KitchenErrorMessage = "An error occurred while retrieving the kitchens.";
        }
    }

    private async Task ListenToGameEvents()
    {
        if (_gameHubConnection is not null)
            await _gameHubConnection.DisposeAsync();

        _gameHubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri($"/GameHub?gameId={GameId}"))
            .Build();

        _gameHubConnection.On(nameof(GamePausedOrResumedEvent),
            async (GamePausedOrResumedEvent @event) =>
            {
                Paused = @event.Paused;
                await InvokeAsync(StateHasChanged);
            });

        _gameHubConnection.On(nameof(MomentPassedEvent),
            async (MomentPassedEvent @event) =>
            {
                TimePassed = @event.TimePassed;
                await InvokeAsync(StateHasChanged);
            });

        _gameHubConnection.On(nameof(GameResetEvent),
            async (GameResetEvent _) =>
            {
                TimePassed = TimeSpan.Zero;
                await InvokeAsync(StateHasChanged);
            });

        try
        {
            await _gameHubConnection.StartAsync();
        }
        catch
        {
            snackbar.Add("Failed to start listening to game events", Severity.Error);
        }
    }

    private async Task PauseOrUnpauseGame()
    {
        Busy = true;
        try
        {
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var pauseOrUnpauseResult = await gameGrain.PauseOrUnpauseGame();

            if (pauseOrUnpauseResult.Succeeded)
            {
                Paused = pauseOrUnpauseResult.Value.Paused;
                snackbar.Add($"Game has been {((bool)Paused ? "paused" : "unpaused")}.", Severity.Success);
            }
            else
            {
                snackbar.Add(pauseOrUnpauseResult.Error.Message, Severity.Error);
            }
        }
        catch
        {
            snackbar.Add("An error occurred while trying to pause or unpause the game.", Severity.Error);
        }
        finally
        {
            Busy = false;
        }
    }

    private async Task NextMoment()
    {
        try
        {
            Busy = true;
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var nextMomentResult = await gameGrain.NextMoment();

            if (!nextMomentResult.Succeeded)
            {
                snackbar.Add(nextMomentResult.Error.Message, Severity.Error);
            }
        }
        catch
        {
            snackbar.Add("An error occurred while trying to advance the game.", Severity.Error);
        }
        finally
        {
            Busy = false;
        }
    }

    private async Task GenerateOrder()
    {
        try
        {
            Busy = true;
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var generateOrderResult = await gameGrain.GenerateOrder();

            if (!generateOrderResult.Succeeded)
            {
                snackbar.Add(generateOrderResult.Error.Message, Severity.Error);
            }
        }
        catch
        {
            snackbar.Add("An error occurred while trying to generate an order.", Severity.Error);
        }
        finally
        {
            Busy = false;
        }
    }

    private async Task Update()
    {
        var dialogParameters = new DialogParameters { { "GameId", GameId } };
        var dialog = await dialogService.ShowAsync<UpdateGameDialog>("Edit Game", dialogParameters);
    }

    private async Task Reset()
    {
        try
        {
            Busy = true;
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var resetGameResult = await gameGrain.ResetGame();

            if (!resetGameResult.Succeeded)
            {
                snackbar.Add(resetGameResult.Error.Message, Severity.Error);
            }
            else
            {
                snackbar.Add("Successfully reset the game.", Severity.Success);
            }
        }
        catch
        {
            snackbar.Add("An error occurred while trying to reset the game.", Severity.Error);
        }
        finally
        {
            Busy = false;
        }
    }
}