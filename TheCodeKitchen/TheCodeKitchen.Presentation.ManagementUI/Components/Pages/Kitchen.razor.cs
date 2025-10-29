using AutoMapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TheCodeKitchen.Application.Contracts.Grains;
using TheCodeKitchen.Presentation.ManagementUI.Models.ViewModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Pages;

public partial class Kitchen(
    NavigationManager navigationManager,
    IDialogService dialogService,
    ISnackbar snackbar,
    IClusterClient clusterClient,
    IMapper mapper
) : ComponentBase
{
    [Parameter] public Guid KitchenId { get; set; }
    private KitchenViewModel? KitchenViewModel { get; set; }
    private string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadKitchen();
        await base.OnInitializedAsync();
    }

    private async Task LoadKitchen()
    {
        try
        {
            KitchenViewModel = null;
            var kitchenGrain = clusterClient.GetGrain<IKitchenGrain>(KitchenId);
            var getKitchenResult = await kitchenGrain.GetKitchen();
            if (getKitchenResult.Succeeded)
            {
                var kitchen = getKitchenResult.Value;
                KitchenViewModel = new KitchenViewModel
                {
                    Id = kitchen.Id,
                    Name = kitchen.Name,
                    Code = kitchen.Code,
                    Rating = kitchen.OrderRatings.Values.DefaultIfEmpty(1.0).Average()
                };
            }
            else
                ErrorMessage = getKitchenResult.Error.Message;
        }
        catch
        {
            ErrorMessage = "An error occurred while retrieving the kitchen.";
        }
    }
}