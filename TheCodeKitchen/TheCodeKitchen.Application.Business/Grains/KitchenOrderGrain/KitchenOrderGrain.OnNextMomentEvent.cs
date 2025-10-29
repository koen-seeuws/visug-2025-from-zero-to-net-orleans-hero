using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.KitchenOrderGrain;

public sealed partial class KitchenOrderGrain
{
    private int _nextMomentCounter;

    private async Task OnNextMomentEvent(NextMomentEvent nextMomentEvent, StreamSequenceToken _)
    {
        if (state.State.Completed)
            return;

        // Time order has been active
        var time = state.State.Time += nextMomentEvent.TimePerMoment;

        // Rating down order when it takes too long
        var nonDeliveredRequestedFoodRatings = state.State.RequestedFoods
            .Where(fr => !fr.Delivered)
            .ToList();

        foreach (var foodRating in nonDeliveredRequestedFoodRatings)
        {
            var margin = foodRating.MinimumTimeToPrepareFood * RatingMargin.WaitingTime;
            var graceTime = foodRating.MinimumTimeToPrepareFood + margin;

            if (time <= graceTime)
                continue; // Still within required time + margin

            var overTime = time - graceTime;
            var penaltyPercent = foodRating.MinimumTimeToPrepareFood > TimeSpan.Zero
                ? overTime / foodRating.MinimumTimeToPrepareFood 
                : 0.0;

            foodRating.Rating = Math.Max(0.0, 1.0 - penaltyPercent); // Rating cannot go below 0
        }

        // Update kitchen rating every 100 moments
        if (nonDeliveredRequestedFoodRatings.Count > 0 && ++_nextMomentCounter % 100 == 0)
        {
            await UpdateKitchenRating();
            _nextMomentCounter = 0;
        }
    }
}