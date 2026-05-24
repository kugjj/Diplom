using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public string? CustomerName { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? Status { get; set; }

    public virtual Product? Product { get; set; }
}
