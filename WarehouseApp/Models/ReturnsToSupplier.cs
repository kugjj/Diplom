using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class ReturnsToSupplier
{
    public int ReturnId { get; set; }

    public int? InboundId { get; set; }

    public string? ReturnReason { get; set; }

    public DateTime? ReturnDate { get; set; }

    public int Quantity { get; set; }

    public virtual InboundInvoice? Inbound { get; set; }
}
