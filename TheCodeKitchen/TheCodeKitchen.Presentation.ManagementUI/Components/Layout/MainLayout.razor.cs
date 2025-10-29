using Microsoft.AspNetCore.Components;
using TheCodeKitchen.Presentation.ManagementUI.Services;

namespace TheCodeKitchen.Presentation.ManagementUI.Components.Layout;

public partial class MainLayout(ClientTimeService clientTimeService) : LayoutComponentBase
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await clientTimeService.InitializeAsync();
        await base.OnAfterRenderAsync(firstRender);
    }
}