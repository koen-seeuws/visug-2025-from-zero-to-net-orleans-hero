namespace TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;

public sealed class CreateRecipeFormModel
{
    public string Name { get; set; } = string.Empty;
    public ICollection<StepFormModel> Steps { get; set; } = new List<StepFormModel>();

    public ICollection<IngredientFormModel> Ingredients { get; set; } = new List<IngredientFormModel>
    {
        new(),
        new()
    };
}

public sealed class IngredientFormModel
{
    public string Name { get; set; } = string.Empty;
    public ICollection<StepFormModel> Steps { get; set; } = new List<StepFormModel>();
}

public sealed class StepFormModel
{
    public string EquipmentType { get; set; } = string.Empty;
    public TimeSpan? Time { get; set; } = TimeSpan.Zero;
}