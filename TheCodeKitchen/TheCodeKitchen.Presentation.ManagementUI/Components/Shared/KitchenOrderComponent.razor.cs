using Microsoft.AspNetCore.Components;
using TheCodeKitchen.Presentation.ManagementUI.Models.ViewModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Shared;

public partial class KitchenOrderComponent : ComponentBase
{
    [Parameter] public KitchenOrderViewModel KitchenOrder { get; set; } = null!;
}