using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - Fridge")]
public sealed class FridgeController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.Fridge);