using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class OutboundInvoice
{
    public int OutboundId { get; set; }

    public int? ProductId { get; set; }

    public int? EmployeeId { get; set; }

    public int? WarehouseId { get; set; }

    public int Quantity { get; set; }

    public decimal? SellingPrice { get; set; }

    public DateTime? DateShipped { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
