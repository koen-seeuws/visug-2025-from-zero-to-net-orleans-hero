using TheCodeKitchen.Application.Contracts.Requests.Equipment;
using TheCodeKitchen.Application.Contracts.Response.Food;

namespace TheCodeKitchen.Application.Contracts.Grains;

public interface IEquipmentGrain : IGrainWithGuidCompoundKey
{
    Task<Result<TheCodeKitchenUnit>> Initialize(CreateEquipmentRequest request);
    Task<Result<TheCodeKitchenUnit>> AddFood(AddFoodRequest request);
    Task<Result<TakeFoodResponse>> TakeFood(TakeFoodFromEquipmentRequest request);
    Task<Result<TheCodeKitchenUnit>> Clean();
}