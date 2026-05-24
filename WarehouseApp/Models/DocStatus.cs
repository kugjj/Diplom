using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class DocStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;
}
