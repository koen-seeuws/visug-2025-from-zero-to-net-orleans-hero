using Microsoft.AspNetCore.Components;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Layout;

public partial class NavMenu(IConfiguration configuration) : ComponentBase
{
    private string? OrleansDashBoardUrl { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        OrleansDashBoardUrl = configuration.GetValue<string>("TheCodeKitchen:OrleansDashboardUrl");
    }
}