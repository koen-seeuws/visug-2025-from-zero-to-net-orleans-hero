using TheCodeKitchen.Application.Contracts.Events.Cook;
using TheCodeKitchen.Application.Contracts.Requests.Cook;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public async Task<Result<TheCodeKitchenUnit>> ReceiveMessage(ReceiveMessageRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError(
                $"The cook with username {this.GetPrimaryKeyString()} does not exist in kitchen {this.GetPrimaryKey()}");
        
        var message = new Message(++state.State.MessageCounter, request.From, request.To, request.Content);
        state.State.Messages.Add(message);

        await state.WriteStateAsync();

        var @event = new MessageReceivedEvent(message.Number, message.From, message.To, message.Content);
        await realTimeCookService.SendMessageReceivedEvent(@event);

        return TheCodeKitchenUnit.Value;
    }
}