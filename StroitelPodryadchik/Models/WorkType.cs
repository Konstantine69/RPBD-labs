using System;
using System.Collections.Generic;
using StroitelPodryadchik.Models;

namespace StroitelPodryadchik;

public partial class WorkType
{
    public int WorkTypeId { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public DateOnly LicenseDate { get; set; }

    public DateOnly LicenseExpirationDate { get; set; }

    public string ClassifierCode { get; set; } = null!;

    public virtual ICollection<ObjectWork> ObjectWorks { get; set; } = new List<ObjectWork>();
}
