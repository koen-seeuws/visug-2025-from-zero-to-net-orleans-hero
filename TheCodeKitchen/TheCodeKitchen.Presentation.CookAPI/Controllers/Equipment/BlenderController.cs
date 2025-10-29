using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - Blender")]
public sealed class BlenderController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.Blender);