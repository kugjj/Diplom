using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class InventoryAct
{
    public int ActId { get; set; }

    public int? WarehouseId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? CheckDate { get; set; }

    public string? Notes { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<InventoryDetail> InventoryDetails { get; set; } = new List<InventoryDetail>();

    public virtual Warehouse? Warehouse { get; set; }
}
