using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public int? WarehouseId { get; set; }
    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<InboundInvoice> InboundInvoices { get; set; } = new List<InboundInvoice>();

    public virtual ICollection<InventoryAct> InventoryActs { get; set; } = new List<InventoryAct>();

    public virtual ICollection<OutboundInvoice> OutboundInvoices { get; set; } = new List<OutboundInvoice>();

    public virtual ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();

    public virtual Warehouse? Warehouse { get; set; }
}
