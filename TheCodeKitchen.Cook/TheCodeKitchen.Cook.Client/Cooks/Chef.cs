using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using TheCodeKitchen.Cook.Client.Custom;
using TheCodeKitchen.Cook.Contracts.Constants;
using TheCodeKitchen.Cook.Contracts.Reponses.CookBook;
using TheCodeKitchen.Cook.Contracts.Reponses.Pantry;
using TheCodeKitchen.Cook.Contracts.Requests.Communication;
using TheCodeKitchen.Cook.Contracts.Requests.Timer;
using Timer = TheCodeKitchen.Cook.Client.Custom.Timer;

namespace TheCodeKitchen.Cook.Client.Cooks;

public class Chef : Cook
{
    private readonly TheCodeKitchenClient _theCodeKitchenClient;

    // Configuration
    private readonly string _headChef;
    private readonly string _equipmentCoordinator;

    // Cached Data
    private ICollection<GetIngredientResponse> _ingredients = new List<GetIngredientResponse>();
    private ICollection<GetRecipeResponse> _recipes = new List<GetRecipeResponse>();

    // Equipment Locking
    private readonly ConcurrentQueue<TaskCompletionSource<MessageContent>> _pendingGrants = new();

    // Food Processing
    private int _foodId = 1;
    private readonly SemaphoreSlim _holdFoodSemaphore = new(1, 1);

    // Food Tracking
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> _preparedIngredients =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentDictionary<string, Tuple<string, int>> _preparedIngredientLocations =
        new(StringComparer.OrdinalIgnoreCase);

    public Chef(string headChef, string equipmentCoordinator, string kitchenCode, string username, string password,
        TheCodeKitchenClient theCodeKitchenClient) : base(theCodeKitchenClient)
    {
        _headChef = headChef;
        _equipmentCoordinator = equipmentCoordinator;
        _theCodeKitchenClient = theCodeKitchenClient;
        Username = username;
        Password = password;
        KitchenCode = kitchenCode;

        OnKitchenOrderCreatedEvent = async kitchenOrderCreatedEvent =>
        {
            // Chefs do not process new orders, only the head chef does
        };

        OnTimerElapsedEvent = async timerElapsedEvent =>
        {
            var timer = new Timer(
                timerElapsedEvent.Number,
                JsonSerializer.Deserialize<TimerNote>(timerElapsedEvent.Note!)
            );

            try
            {
                await ProcessTimerElapsedEvent(timer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{Username} - Error processing timer elapsed event {JsonSerializer.Serialize(timer)}: {ex}");
                Console.WriteLine($"{Username} - Completing order {timer.Note.Order}");
                await _theCodeKitchenClient
                    .CompleteOrder(timer.Note.Order); // In case something goes wrong with food in this order, just skip it
            }
        };

        OnMessageReceivedEvent = async messageReceivedEvent =>
        {
            var message = new Message(
                messageReceivedEvent.Number,
                messageReceivedEvent.From,
                messageReceivedEvent.To,
                JsonSerializer.Deserialize<MessageContent>(messageReceivedEvent.Content)!
            );

            try
            {
                await ProcessMessage(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{Username} - Error processing message {JsonSerializer.Serialize(message)}: {ex}");
                if (message.Content.Order.HasValue)
                {
                    Console.WriteLine($"{Username} - Completing order {message.Content.Order!.Value}");
                    await _theCodeKitchenClient.CompleteOrder(message.Content.Order
                        .Value); // In case something goes wrong with food in this order, just skip it
                }
            }
        };
    }

    public override async Task StartCooking(CancellationToken cancellationToken = default)
    {
        await base.StartCooking(cancellationToken);

        _recipes = await _theCodeKitchenClient.ReadRecipes(cancellationToken);
        _ingredients = await _theCodeKitchenClient.PantryInventory(cancellationToken);
    }

    private async Task ProcessMessage(Message message)
    {
        switch (message.Content.Code)
        {
            case MessageCodes.GrantEquipment:
            {
                Console.WriteLine(
                    $"{Username} - Process Message - Granted Equipment - {message.Content.EquipmentType} {message.Content.EquipmentNumber}");

                if (_pendingGrants.TryDequeue(out var pendingGrant))
                {
                    pendingGrant.SetResult(message.Content);
                }
                else
                {
                    Console.WriteLine($"{Username} - WARNING: Received grant but no pending request!");
                }

                break;
            }
            case MessageCodes.CookFood:
            {
                Console.WriteLine(
                    $"{Username} - Process Message - Cooking Food - Order: {message.Content.Order!.Value}, Food: {message.Content.Food!}");
                var foodId = Interlocked.Increment(ref _foodId);
                await StartCookingIngredients(message.Content.Order!.Value, message.Content.Food!, [], foodId);
                break;
            }
        }

        var confirmMessageRequest = new ConfirmMessageRequest(message.Number);
        await _theCodeKitchenClient.ConfirmMessage(confirmMessageRequest);
    }

    private async Task ReleaseEquipment(string equipmentType, int number)
    {
        Console.WriteLine($"{Username} - Releasing Equipment - {equipmentType} {number}");
        var release = new MessageContent(MessageCodes.ReleaseEquipment, null, null, equipmentType, number);
        await _theCodeKitchenClient.SendMessage(new SendMessageRequest(_equipmentCoordinator,
            JsonSerializer.Serialize(release)));
    }

    private async Task<int> RequestEquipment(string equipmentType)
    {
        var pendingGrant = new TaskCompletionSource<MessageContent>();

        // Add to queue before sending request
        _pendingGrants.Enqueue(pendingGrant);

        var request = new MessageContent(MessageCodes.RequestEquipment, null, null, equipmentType, null);
        await _theCodeKitchenClient.SendMessage(new SendMessageRequest(_equipmentCoordinator,
            JsonSerializer.Serialize(request)));

        // Wait for GrantEquipment for this chef (wait until coordinator responds)
        var grant = await pendingGrant.Task;

        return grant.EquipmentNumber!.Value;
    }

    private async Task SetTimer(long orderNumber, string food, string[] recipeTree, RecipeStepDto[] stepsToDo, int id,
        string? equipmentType, int? equipmentNumber, TimeSpan duration)
    {
        var note = new TimerNote(
            orderNumber,
            food,
            recipeTree,
            equipmentType,
            equipmentNumber,
            stepsToDo,
            id
        );
        var jsonNote = JsonSerializer.Serialize(note);
        var setTimerRequest = new SetTimerRequest(duration, jsonNote);
        await _theCodeKitchenClient.SetTimer(setTimerRequest);
    }

    private async Task StartCookingIngredients(long orderNumber, string food, string[] ingredientOfTree, int id)
    {
        var recipe = _recipes.First(r => r.Name.Equals(food, StringComparison.OrdinalIgnoreCase));

        foreach (var ingredient in recipe.Ingredients)
        {
            var isRecipe = _recipes.Any(r => r.Name.Equals(ingredient.Name, StringComparison.OrdinalIgnoreCase));
            if (isRecipe)
            {
                // Ingredient is a recipe -> Recursively process it
                await StartCookingIngredients(orderNumber, ingredient.Name, ingredientOfTree.Append(food).ToArray(),
                    id);
            }
            else
            {
                // Ingredient is a base ingredient -> Cook the food
                await CookFood(orderNumber, ingredient.Name, ingredientOfTree.Append(food).ToArray(),
                    ingredient.Steps.ToArray(), id, null, null);
            }
        }
    }

    private async Task CookFood(long orderNumber, string food, string[] recipeTree, RecipeStepDto[] stepsToDo, int id,
        string? sourceEquipmentType, int? sourceEquipmentNumber)
    {
        var allStepsDone = !stepsToDo.Any();

        // Not all steps done -> Simply put it in the next necessary equipment
        if (!allStepsDone)
        {
            var stepToDo = stepsToDo.First();
            var destinationEquipmentType = stepToDo.EquipmentType;
            var destinationEquipmentNumber = await RequestEquipment(destinationEquipmentType);
            if (destinationEquipmentNumber < 0)
            {
                // No equipment available -> Try again in 2 minutes
                await SetTimer(orderNumber, food, recipeTree, stepsToDo, id, sourceEquipmentType, sourceEquipmentNumber,
                    TimeSpan.FromMinutes(2));
                return;
            }

            await _holdFoodSemaphore.WaitAsync();
            try
            {
                if (sourceEquipmentType != null && sourceEquipmentNumber.HasValue)
                {
                    // Food is in equipment -> Take from there
                    await TakeFoodFromEquipment(sourceEquipmentType, sourceEquipmentNumber.Value);
                    await AddFoodToEquipment(destinationEquipmentType, destinationEquipmentNumber);
                    await ReleaseEquipment(sourceEquipmentType, sourceEquipmentNumber.Value);
                }
                else
                {
                    // Food is in pantry -> Take from there
                    await TakeFoodFromPantry(food);
                    await AddFoodToEquipment(destinationEquipmentType, destinationEquipmentNumber);
                }
            }
            finally
            {
                _holdFoodSemaphore.Release();
            }

            await SetTimer(
                orderNumber,
                food,
                recipeTree,
                stepsToDo.Skip(1).ToArray(),
                id,
                destinationEquipmentType,
                destinationEquipmentNumber,
                stepToDo.Time
            );
            return;
        }

        // All steps are done and the food is not part of a recipe -> Notify head chef that food is ready
        var isRequestedByHeadChef = recipeTree.Length <= 0;
        if (isRequestedByHeadChef)
        {
            var messageContent = new MessageContent(
                MessageCodes.FoodReady,
                orderNumber,
                food,
                sourceEquipmentType,
                sourceEquipmentNumber
            );
            var sendMessage = new SendMessageRequest(_headChef, JsonSerializer.Serialize(messageContent));
            await _theCodeKitchenClient.SendMessage(sendMessage);
            return;
        }

        // All steps are done, but is part of recipe
        var recipeName = recipeTree.Last();
        var recipe = _recipes.First(r => r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));

        var recipeKey = $"{id}_{string.Join('_', recipeTree)}".ToUpper();

        var preparedIngredients =
            _preparedIngredients.GetOrAdd(recipeKey, _ => new ConcurrentDictionary<string, bool>());

        // Tracking prepared ingredients
        preparedIngredients.TryAdd(food, true);

        // Track location for ANY ingredient that has equipment (either has steps OR is a sub-recipe)
        if (sourceEquipmentType != null && sourceEquipmentNumber.HasValue)
        {
            // This ingredient/recipe is currently in equipment, we need to track its location
            var ingredientsToBePreparedWithSteps = recipe.Ingredients.Where(i =>
                i.Steps.Count > 0 || _recipes.Any(r => r.Name.Equals(i.Name, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            if (ingredientsToBePreparedWithSteps.Select(i => i.Name).Contains(food, StringComparer.OrdinalIgnoreCase))
            {
                // Current ingredient/recipe needs to be held for later merging
                var destinationEquipmentType = EquipmentType.Counter;
                var destinationEquipmentNumber = await RequestEquipment(destinationEquipmentType);

                if (destinationEquipmentNumber < 0)
                {
                    destinationEquipmentType = EquipmentType.HotPlate;
                    destinationEquipmentNumber = await RequestEquipment(destinationEquipmentType);
                }

                if (destinationEquipmentNumber < 0)
                {
                    // No Counter or Hot Plate available -> Try again in 2 minutes
                    await SetTimer(orderNumber, food, recipeTree, stepsToDo, id, sourceEquipmentType,
                        sourceEquipmentNumber,
                        TimeSpan.FromMinutes(2));
                    return;
                }

                // Lock destination equipment, take food, unlock source equipment, add food to destination equipment
                await _holdFoodSemaphore.WaitAsync();
                try
                {
                    await TakeFoodFromEquipment(sourceEquipmentType, sourceEquipmentNumber!.Value);
                    await AddFoodToEquipment(destinationEquipmentType, destinationEquipmentNumber);
                    await ReleaseEquipment(sourceEquipmentType, sourceEquipmentNumber.Value);
                }
                finally
                {
                    _holdFoodSemaphore.Release();
                }

                // Track the prepared ingredient/recipe location
                var location = new Tuple<string, int>(destinationEquipmentType, destinationEquipmentNumber);
                var locationKey = $"{recipeKey}_{food}".ToUpper();
                _preparedIngredientLocations[locationKey] = location;
            }
        }

        // Merging into recipe
        if (recipe.Ingredients.Count == preparedIngredients.Count)
        {
            // All ingredients with steps are ready, take the prepared ingredients, take the ingredients without steps from pantry and merge into recipe
            var firstRecipeStep = recipe.Steps.FirstOrDefault();
            var destinationEquipmentType = firstRecipeStep?.EquipmentType ?? EquipmentType.Counter;
            var destinationEquipmentNumber = await RequestEquipment(destinationEquipmentType);

            if (destinationEquipmentType == EquipmentType.Counter && destinationEquipmentNumber < 0)
            {
                destinationEquipmentType = EquipmentType.HotPlate;
                destinationEquipmentNumber = await RequestEquipment(destinationEquipmentType);
            }

            if (destinationEquipmentNumber < 0)
            {
                // No Counter or Hot Plate available -> Try again in 2 minutes
                await SetTimer(
                    orderNumber,
                    food,
                    recipeTree,
                    stepsToDo,
                    id,
                    sourceEquipmentType,
                    sourceEquipmentNumber,
                    TimeSpan.FromMinutes(2)
                );
                return;
            }

            // NOW it's safe to remove from tracking, as we have the equipment
            if (!_preparedIngredients.TryRemove(recipeKey, out var ingredientsToMerge))
            {
                // Another thread already handled this recipe merge
                await ReleaseEquipment(destinationEquipmentType, destinationEquipmentNumber);
                return;
            }

            await _holdFoodSemaphore.WaitAsync();
            try
            {
                foreach (var preparedIngredient in ingredientsToMerge)
                {
                    var locationKey = $"{recipeKey}_{preparedIngredient.Key}".ToUpper();
                    var locationExists = _preparedIngredientLocations.TryGetValue(locationKey, out var location);
                    if (locationExists)
                    {
                        await TakeFoodFromEquipment(location!.Item1, location.Item2);
                        await AddFoodToEquipment(destinationEquipmentType, destinationEquipmentNumber);
                        await ReleaseEquipment(location.Item1, location.Item2);
                        _preparedIngredientLocations.TryRemove(locationKey, out _);
                    }
                    else
                    {
                        await TakeFoodFromPantry(preparedIngredient.Key);
                        await AddFoodToEquipment(destinationEquipmentType, destinationEquipmentNumber);
                    }
                }
            }
            finally
            {
                _holdFoodSemaphore.Release();
            }

            if (firstRecipeStep != null)
            {
                await SetTimer(
                    orderNumber,
                    recipe.Name,
                    recipeTree.SkipLast(1).ToArray(),
                    recipe.Steps.Skip(1).ToArray(),
                    id,
                    destinationEquipmentType,
                    destinationEquipmentNumber,
                    firstRecipeStep.Time
                );
            }
            else
            {
                await CookFood(
                    orderNumber,
                    recipe.Name,
                    recipeTree.SkipLast(1).ToArray(),
                    recipe.Steps.Skip(1).ToArray(),
                    id,
                    destinationEquipmentType,
                    destinationEquipmentNumber
                );
            }
        }
    }

    private async Task ProcessTimerElapsedEvent(Timer timer)
    {
        Console.WriteLine($"{Username} - Process Timer Elapsed Event - {JsonSerializer.Serialize(timer)}");
        var timerNote = timer.Note;
        await CookFood(timerNote.Order, timerNote.Food, timerNote.RecipeTree, timerNote.StepsToDo, timerNote.Id,
            timerNote.EquipmentType, timerNote.EquipmentNumber);
        var stopTimerRequest = new StopTimerRequest(timer.Number);
        await _theCodeKitchenClient.StopTimer(stopTimerRequest);
    }

    private async Task TakeFoodFromPantry(string ingredient)
    {
        Console.WriteLine($"{Username} - Take Food From Pantry - {ingredient}");
        await _theCodeKitchenClient.TakeFoodFromPantry(ingredient);
    }

    private async Task TakeFoodFromEquipment(string equipmentType, int number)
    {
        Console.WriteLine($"{Username} - Take Food From Equipment - {equipmentType} {number}");
        await _theCodeKitchenClient.TakeFoodFromEquipment(equipmentType, number);
    }

    private async Task AddFoodToEquipment(string equipmentType, int number)
    {
        Console.WriteLine($"{Username} - Add Food To Equipment - {equipmentType} {number}");
        await _theCodeKitchenClient.AddFoodToEquipment(equipmentType, number);
    }
}