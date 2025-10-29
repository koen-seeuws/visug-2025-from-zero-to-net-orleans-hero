namespace TheCodeKitchen.Application.Contracts.Models.Recipe;

[GenerateSerializer]
public record RecipeStepDto(string EquipmentType, TimeSpan Time);