using TheCodeKitchen.Application.Contracts.Requests.Game;
using TheCodeKitchen.Application.Contracts.Response.Game;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<CreateGameResponse>> Initialize(CreateGameRequest request, int count)
    {
        if (state.RecordExists)
            return new AlreadyExistsError($"The game with id {this.GetPrimaryKey()} has already been initialized");

        var id = this.GetPrimaryKey();
        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            name = $"Game {count}";

        var game = new Game(
            id,
            name,
            request.TimePerMoment,
            request.MinimumTimeBetweenOrders,
            request.MaximumTimeBetweenOrders,
            request.SpeedModifier,
            request.MinimumItemsPerOrder,
            request.MaximumItemsPerOrder,
            request.OrderSpeedModifier,
            request.Temperature
        );
        state.State = game;
        await state.WriteStateAsync();

        return mapper.Map<CreateGameResponse>(game);
    }
}