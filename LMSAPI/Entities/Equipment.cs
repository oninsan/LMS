using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class Equipment
{
    public int Id { get; set; }

    public string? EquipmentName { get; set; }

    public int? Quantity { get; set; }

    public bool? DamageStatus { get; set; }
}
