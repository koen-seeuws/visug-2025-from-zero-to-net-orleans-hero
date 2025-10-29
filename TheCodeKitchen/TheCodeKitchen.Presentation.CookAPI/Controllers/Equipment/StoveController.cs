using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - Stove")]
public sealed class StoveController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.Stove);