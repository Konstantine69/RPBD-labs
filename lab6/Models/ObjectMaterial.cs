using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab6.Models;

public partial class ObjectMaterial
{
    public int ObjectMaterialId { get; set; }

    public int ObjectId { get; set; }

    public int MaterialId { get; set; }

    [ForeignKey("MaterialId")]
    public virtual BuildingMaterial? Material { get; set; } = null!;

    [ForeignKey("ObjectId")]
    public virtual ConstructionObject? Object { get; set; } = null!;
}
