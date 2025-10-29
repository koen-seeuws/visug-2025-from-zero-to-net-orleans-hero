using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Constants;

namespace TheCodeKitchen.Presentation.API.Cook.Controllers.Equipment;

[Tags("Equipment - BBQ")]
[Route("Equipment/BBQ")]
public sealed class BbqController(IClusterClient clusterClient) : EquipmentController(clusterClient, EquipmentType.Bbq);