using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class PriceHistory
{
    public int HistoryId { get; set; }

    public int? ProductId { get; set; }

    public decimal? OldPrice { get; set; }

    public decimal? NewPrice { get; set; }

    public DateTime? ChangeDate { get; set; }

    public virtual Product? Product { get; set; }
}
