using System;
using System.Collections.Generic;

namespace StroitelPodryadchik.Models;

public partial class ConstructionObject
{
    public int ObjectId { get; set; }

    public string ObjectName { get; set; } = null!;

    public int CustomerId { get; set; }

    public string GeneralContractor { get; set; } = null!;

    public DateOnly ContractDate { get; set; }

    public string? WorkList { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public DateOnly? CommissioningDate { get; set; }

    public string? Photo { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<ObjectMaterial> ObjectMaterials { get; set; } = new List<ObjectMaterial>();

    public virtual ICollection<ObjectWork> ObjectWorks { get; set; } = new List<ObjectWork>();
}
