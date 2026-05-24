using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class InboundInvoice
{
    public int InboundId { get; set; }

    public int? SupplierId { get; set; }

    public int? ProductId { get; set; }

    public int? EmployeeId { get; set; }

    public int? WarehouseId { get; set; }

    public int Quantity { get; set; }

    public decimal? PurchasePrice { get; set; }

    public DateTime? DateReceived { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ICollection<ReturnsToSupplier> ReturnsToSuppliers { get; set; } = new List<ReturnsToSupplier>();

    public virtual Supplier? Supplier { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
