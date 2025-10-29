using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - Oven")]
public sealed class OvenController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.Oven);