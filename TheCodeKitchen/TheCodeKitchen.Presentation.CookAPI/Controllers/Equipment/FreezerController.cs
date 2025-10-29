using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - Freezer")]
public sealed class FreezerController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.Freezer);