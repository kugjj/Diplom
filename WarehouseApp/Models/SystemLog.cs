using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class SystemLog
{
    public int LogId { get; set; }

    public int? EmployeeId { get; set; }

    public string? ActionDescription { get; set; }

    public DateTime? ActionTime { get; set; }

    public virtual Employee? Employee { get; set; }
}
