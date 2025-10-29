namespace TheCodeKitchen.Presentation.ManagementUI.Models.ViewModels;

public sealed class KitchenOrderViewModel
{
    public long Number { get; set; }
    public required ICollection<string> RequestedFoods { get; set; }
    public required ICollection<string> DeliveredFoods { get; set; }
}