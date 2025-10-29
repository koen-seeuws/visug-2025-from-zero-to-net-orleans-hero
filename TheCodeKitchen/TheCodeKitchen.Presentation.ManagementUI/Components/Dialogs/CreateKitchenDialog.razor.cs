using Microsoft.AspNetCore.Components;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;
using TheCodeKitchen.Presentation.ManagementUI.Validation;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Dialogs;

public partial class CreateKitchenDialog(
    ISnackbar snackbar,
    IClusterClient clusterClient,
    CreateKitchenFormModelValidator validator
) : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public Guid GameId { get; set; }
    private MudForm Form { get; set; } = new();
    private CreateKitchenFormModel Model { get; set; } = new();
    private bool Creating { get; set; }

    private async Task Submit()
    {
        await Form.Validate();
        if (!Form.IsValid)
            return;

        Creating = true;
        try
        {
            var request = new CreateKitchenRequest(Model.Name, GameId);
            var gameGrain = clusterClient.GetGrain<IGameGrain>(GameId);
            var createKitchenResult = await gameGrain.CreateKitchen(request);

            if (createKitchenResult.Succeeded)
            {
                snackbar.Add("Successfully created kitchen.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(createKitchenResult.Value));
            }

            else
                snackbar.Add(createKitchenResult.Error.Message, Severity.Error);
        }
        catch
        {
            snackbar.Add("An error occured while trying to create a kitchen.", Severity.Error);
        }
        finally
        {
            Creating = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}