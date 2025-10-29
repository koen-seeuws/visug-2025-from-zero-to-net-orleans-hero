using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Response.Cook;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public async Task<Result<CreateCookResponse>> Initialize(CreateCookRequest request)
    {
        if (state.RecordExists)
            return new AlreadyExistsError(
                $"The cook with username {this.GetPrimaryKeyString()} has already been initialized in kitchen {this.GetPrimaryKey()}");

        var username = request.Username.Trim();

        var cook = new Cook(username, request.PasswordHash, request.Game, request.Kitchen);
        state.State = cook;
        await state.WriteStateAsync();

        await SubscribeToNextMomentEvent();

        return mapper.Map<CreateCookResponse>(cook);
    }
}