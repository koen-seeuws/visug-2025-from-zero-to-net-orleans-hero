using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Contracts.Events.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    public async Task<Result<TheCodeKitchenUnit>> ResetKitchen()
    {
        var resetCookTasks = state.State.Cooks
            .Select(async cook =>
            {
                var cookGrain = GrainFactory.GetGrain<ICookGrain>(state.State.Id, cook);
                var resetCookResult = await cookGrain.Reset();
                return resetCookResult;
            });
        
        var cleanEquipmentTasks = state.State.Equipment
            .SelectMany(e =>
            {
                var cleanEquipmentTypeTasks = Enumerable
                    .Range(0, e.Value)
                    .Select(async number =>
                    {
                        var equipmentGrainIdExtension = EquipmentGrainId.Create(e.Key, number);
                        var equipmentGrain = GrainFactory.GetGrain<IEquipmentGrain>(state.State.Id, equipmentGrainIdExtension);
                        var cleanEquipmentResult = await equipmentGrain.Clean();
                        return cleanEquipmentResult;
                    });

                return cleanEquipmentTypeTasks;
            });
        
        var cancelOrdersTasks = state.State.Orders
            .Select(async orderNumber =>
            {
                var kitchenOrderGrain = GrainFactory.GetGrain<IKitchenOrderGrain>(orderNumber, state.State.Id.ToString());
                var cancelOrderResult = await kitchenOrderGrain.Cancel();
                return cancelOrderResult;
            });
        
        var resetTasks = resetCookTasks
            .Concat(cleanEquipmentTasks)
            .Concat(cancelOrdersTasks);
        
        var resetResults = await Task.WhenAll(resetTasks);
        var resetResult = resetResults.Combine();
        
        if (!resetResult.Succeeded)
            return resetResult.Error;
        
        state.State.Orders.Clear();
        state.State.OpenOrders.Clear();
        state.State.OrderRatings.Clear();
        await state.WriteStateAsync();
        
        var @event = new KitchenResetEvent();
        await realTimeKitchenService.SendKitchenResetEvent(state.State.Id, @event);
        
        return TheCodeKitchenUnit.Value;
    }
}