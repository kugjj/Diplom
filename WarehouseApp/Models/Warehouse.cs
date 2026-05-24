using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = null!;

    public string? Location { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<InboundInvoice> InboundInvoices { get; set; } = new List<InboundInvoice>();

    public virtual ICollection<InventoryAct> InventoryActs { get; set; } = new List<InventoryAct>();

    public virtual ICollection<OutboundInvoice> OutboundInvoices { get; set; } = new List<OutboundInvoice>();
}
