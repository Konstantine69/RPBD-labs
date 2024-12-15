using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace lab6.Models;

public partial class WorkType
{
    public int WorkTypeId { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public DateOnly LicenseDate { get; set; }

    public DateOnly LicenseExpirationDate { get; set; }

    public string ClassifierCode { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<ObjectWork> ObjectWorks { get; set; } = new List<ObjectWork>();
}
