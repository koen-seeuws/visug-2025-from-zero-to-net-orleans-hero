using TheCodeKitchen.Application.Contracts.Requests.Cook;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public async Task<Result<TheCodeKitchenUnit>> ConfirmMessage(ConfirmMessageRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError(
                $"The cook with username {this.GetPrimaryKeyString()} does not exist in kitchen {this.GetPrimaryKey()}");

        var message = state.State.Messages.FirstOrDefault(m => m.Number == request.Number);

        if (message is not null)
        {
            state.State.Messages.Remove(message);
            await state.WriteStateAsync();
        }

        if (request.Number > state.State.MessageCounter)
            return new NotFoundError($"You don't have a message with number {request.Number} addressed to you");
        
        return TheCodeKitchenUnit.Value; // The message once existed
    }
}