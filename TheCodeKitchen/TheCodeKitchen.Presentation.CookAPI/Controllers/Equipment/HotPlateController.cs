using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - Hot Plate")]
public sealed class HotPlateController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.HotPlate);