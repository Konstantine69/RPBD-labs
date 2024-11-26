using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdajnikWeb.Models;

public partial class ObjectWork
{
    public int ObjectWorkId { get; set; }

    public int ObjectId { get; set; }

    public int WorkTypeId { get; set; }

    [ForeignKey("ObjectId")]
    public virtual ConstructionObject? Object { get; set; } = null!;

    [ForeignKey("WorkTypeId")]
    public virtual WorkType? WorkType { get; set; } = null!;
}
