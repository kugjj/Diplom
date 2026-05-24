using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? Inn { get; set; }

    public string? ContactPhone { get; set; }

    public virtual ICollection<InboundInvoice> InboundInvoices { get; set; } = new List<InboundInvoice>();
}
