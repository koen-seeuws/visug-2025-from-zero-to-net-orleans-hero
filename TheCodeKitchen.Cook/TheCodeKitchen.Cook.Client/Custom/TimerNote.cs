using TheCodeKitchen.Cook.Contracts.Reponses.CookBook;

namespace TheCodeKitchen.Cook.Client.Custom;

public record TimerNote(
    long Order,
    string Food,
    string[] RecipeTree,
    string? EquipmentType,
    int? EquipmentNumber, 
    RecipeStepDto[] StepsToDo,
    int Id
);