using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Business.Helpers;
using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Requests.KitchenOrder;

namespace TheCodeKitchen.Application.Business.Grains.KitchenOrderGrain;

public sealed partial class KitchenOrderGrain
{
    public async Task<Result<TheCodeKitchenUnit>> DeliverFood(DeliverFoodRequest request)
    {
        if (!state.RecordExists)
        {
            var orderNumber = this.GetPrimaryKeyLong();
            var kitchenId = Guid.Parse(this.GetPrimaryKeyString().Split('+')[1]);
            return new NotFoundError($"The order with number {orderNumber} does not exist in kitchen {kitchenId}");
        }

        if (state.State.Completed)
            return new OrderAlreadyCompletedError(
                $"The order with number {state.State.Number} has already been completed, you cannot deliver any more food to it");

        var cookGrain = GrainFactory.GetGrain<ICookGrain>(state.State.Kitchen, request.Cook);
        var releaseFoodResult = await cookGrain.ReleaseFood();
        if (!releaseFoodResult.Succeeded)
            return releaseFoodResult.Error;
        
        var food = mapper.Map<Food>(releaseFoodResult.Value.Food);

        // Rating the delivered food quality
        var cookbookGrain = GrainFactory.GetGrain<ICookBookGrain>(Guid.Empty);
        var getRecipeResult = await cookbookGrain.GetRecipes();
        if (!getRecipeResult.Succeeded)
            return getRecipeResult.Error;
        
        var recipes = mapper.Map<List<Recipe>>(getRecipeResult.Value);

        var qualityRating = RatingHelper.RateFood(food.Name, food.Steps, food.Ingredients, recipes);

        var foodDelivery = new KitchenOrderFoodDelivery(food, qualityRating);
        state.State.DeliveredFoods.Add(foodDelivery);

        // Making sure the customer is no longer waiting for its food (wont be picked up anymore OnNextMoment)
        var foodRequestRating = state.State.RequestedFoods
            .Where(d => !d.Delivered)
            .FirstOrDefault(d => d.Food.Equals(food.Name, StringComparison.OrdinalIgnoreCase));

        if (foodRequestRating is not null)
            foodRequestRating.Delivered = true;

        // Rating order completeness
        var requestedFoods = state.State.RequestedFoods
            .Select(d => d.Food)
            .ToList();

        var deliveredFoods = state.State.DeliveredFoods
            .Select(d => d.Food.Name)
            .ToList();

        var missingFoods = requestedFoods.MultiExcept(deliveredFoods).ToList();
        var wrongFoods = deliveredFoods.MultiExcept(requestedFoods).ToList();
        var correctFoods = requestedFoods.MultiIntersect(deliveredFoods).ToList();

        // In case of missing foods, we apply a penalty for wrong foods
        // In case of no missing food (and possibly extra food), we apply no penalty
        var penaltyWeight = (double) missingFoods.Count / requestedFoods.Count;

        var adjustedCorrectCount = correctFoods.Count - wrongFoods.Count * penaltyWeight;
        adjustedCorrectCount = Math.Max(0, adjustedCorrectCount); // Avoid negative score

        state.State.CompletenessRating = adjustedCorrectCount / requestedFoods.Count;

        // Update state
        await state.WriteStateAsync();
        
        // Update kitchen rating
        await UpdateKitchenRating();
        
        var @event = new KitchenOrderFoodDeliveredEvent(state.State.Number, food.Name, qualityRating);
        await realTimeKitchenOrderService.SendKitchenOrderFoodDeliveredEvent(state.State.Kitchen, @event);

        return TheCodeKitchenUnit.Value;
    }
}