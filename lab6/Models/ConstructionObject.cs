using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lab6.Models;

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

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<ObjectMaterial> ObjectMaterials { get; set; } = new List<ObjectMaterial>();
   
    [JsonIgnore]
    public virtual ICollection<ObjectWork> ObjectWorks { get; set; } = new List<ObjectWork>();
}
