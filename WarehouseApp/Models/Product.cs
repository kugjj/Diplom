using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Sku { get; set; }

    public string ProductName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public int? UnitId { get; set; }

    public decimal? Price { get; set; }

    public int? CurrentStock { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<InboundInvoice> InboundInvoices { get; set; } = new List<InboundInvoice>();

    public virtual ICollection<InventoryDetail> InventoryDetails { get; set; } = new List<InventoryDetail>();

    public virtual ICollection<OutboundInvoice> OutboundInvoices { get; set; } = new List<OutboundInvoice>();

    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Unit? Unit { get; set; }
}
