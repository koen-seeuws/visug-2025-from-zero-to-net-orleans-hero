using Microsoft.JSInterop;

namespace TheCodeKitchen.Presentation.ManagementUI.Services;

public sealed class ClientTimeService(IJSRuntime js)
{
    private TimeSpan Offset { get; set; } = TimeSpan.Zero;

    public async Task InitializeAsync()
    {
        var offsetMinutes = await js.InvokeAsync<int>("timeHelper.getOffsetMinutes");
        Offset = TimeSpan.FromMinutes(offsetMinutes);
    }

    public DateTime? GetLocalDateTime(DateTimeOffset? dateTimeOffset)
    {
        if(!dateTimeOffset.HasValue) 
            return null;
        var afterOffset = dateTimeOffset.Value.ToOffset(Offset);
        return afterOffset.LocalDateTime;
    }
}