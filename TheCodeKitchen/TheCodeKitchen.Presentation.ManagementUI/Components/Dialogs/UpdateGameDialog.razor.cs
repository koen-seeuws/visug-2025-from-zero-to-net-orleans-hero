using Microsoft.AspNetCore.Components;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Game;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;
using TheCodeKitchen.Presentation.ManagementUI.Validation;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Dialogs;

public partial class UpdateGameDialog(
    ISnackbar snackbar,
    IClusterClient clusterClient,
    UpdateGameFormModelValidator validator
) : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public Guid GameId { get; set; }
    public string? ErrorMessage { get; set; }
    private UpdateGameFormModel? Model { get; set; }
    private MudForm Form { get; set; } = new();
    private bool Updating { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
        var getGameResult = await gameGrain.GetGame();
        if(getGameResult.Succeeded)
            Model = new UpdateGameFormModel
            {
                TimePerMoment = getGameResult.Value.TimePerMoment,
                SpeedModifier = getGameResult.Value.SpeedModifier,
                MinimumTimeBetweenOrders = getGameResult.Value.MinimumTimeBetweenOrders,
                MaximumTimeBetweenOrders = getGameResult.Value.MaximumTimeBetweenOrders,
                MinimumItemsPerOrder = getGameResult.Value.MinimumItemsPerOrder,
                MaximumItemsPerOrder = getGameResult.Value.MaximumItemsPerOrder,
                OrderSpeedModifier = getGameResult.Value.OrderSpeedModifier,
                Temperature = getGameResult.Value.Temperature
            };
        else
        {
            ErrorMessage = "An error occurred while retrieving the game details.";
        }
        
        await base.OnInitializedAsync();
    }

    private async Task Submit()
    {
        if(Model == null)
            return;
        
        await Form.Validate();
        if (!Form.IsValid)
            return;

        Updating = true;
        try
        {
            var request = new UpdateGameRequest(
                Model.TimePerMoment!.Value,
                Model.SpeedModifier,
                Model.MinimumTimeBetweenOrders!.Value,
                Model.MaximumTimeBetweenOrders!.Value,
                Model.MinimumItemsPerOrder,
                Model.MaximumItemsPerOrder,
                Model.OrderSpeedModifier,
                Model.Temperature
            );
            var gameManagementGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var createGameResult = await gameManagementGrain.UpdateGame(request);

            if (createGameResult.Succeeded)
            {
                snackbar.Add("Successfully updated game.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(createGameResult.Value));
            }
            else
                snackbar.Add(createGameResult.Error.Message, Severity.Error);
        }
        catch
        {
            snackbar.Add("An error occured while trying to update the game.", Severity.Error);
        }
        finally
        {
            Updating = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}