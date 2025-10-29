using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - Counter")]
public sealed class CounterController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.Counter);