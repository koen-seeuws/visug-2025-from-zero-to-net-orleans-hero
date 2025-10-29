using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - Cutting Board")]
public sealed class CuttingBoardController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.CuttingBoard);