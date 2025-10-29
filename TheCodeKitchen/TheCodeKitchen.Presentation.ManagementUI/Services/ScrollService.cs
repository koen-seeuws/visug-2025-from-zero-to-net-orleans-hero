using Microsoft.JSInterop;

namespace TheCodeKitchen.Presentation.ManagementUI.Services;

public sealed class ScrollService(IJSRuntime jsRuntime)
{
    public async Task ScrollToBottom(string containerId)
    {
        await jsRuntime.InvokeVoidAsync("scrollHelper.scrollToBottom", containerId);
    }

    public async Task ScrollToBottomIfPreviouslyNearBottom(string containerId, int margin = 25)
    {
        await jsRuntime.InvokeVoidAsync("scrollHelper.scrollToBottomIfPreviouslyNearBottom", containerId, margin);
    }
}