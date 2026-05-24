using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class InventoryDetail
{
    public int DetailId { get; set; }

    public int? ActId { get; set; }

    public int? ProductId { get; set; }

    public int? SystemQty { get; set; }

    public int? FactQty { get; set; }

    public int? DiffQty { get; set; }

    public virtual InventoryAct? Act { get; set; }

    public virtual Product? Product { get; set; }
}
