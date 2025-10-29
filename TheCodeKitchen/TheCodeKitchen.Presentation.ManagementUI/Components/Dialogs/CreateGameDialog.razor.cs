using Microsoft.AspNetCore.Components;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Game;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;
using TheCodeKitchen.Presentation.ManagementUI.Validation;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Dialogs;

public partial class CreateGameDialog(
    ISnackbar snackbar,
    IClusterClient clusterClient,
    CreateGameFormModelValidator validator
) : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    private MudForm Form { get; set; } = new();
    private CreateGameFormModel Model { get; set; } = new();
    private bool Creating { get; set; }

    private async Task Submit()
    {
        await Form.Validate();
        if (!Form.IsValid)
            return;

        Creating = true;
        try
        {
            var request = new CreateGameRequest(
                Model.Name,
                Model.TimePerMoment!.Value,
                Model.SpeedModifier,
                Model.MinimumTimeBetweenOrders!.Value,
                Model.MaximumTimeBetweenOrders!.Value,
                Model.MinimumItemsPerOrder,
                Model.MaximumItemsPerOrder,
                Model.OrderSpeedModifier,
                Model.Temperature
            );
            var gameManagementGrain = clusterClient.GetGrain<IGameManagementGrain>(Guid.Empty);
            var createGameResult = await gameManagementGrain.CreateGame(request);

            if (createGameResult.Succeeded)
            {
                snackbar.Add("Successfully created game.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(createGameResult.Value));
            }
            else
                snackbar.Add(createGameResult.Error.Message, Severity.Error);
        }
        catch
        {
            snackbar.Add("An error occured while trying to create a game.", Severity.Error);
        }
        finally
        {
            Creating = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}