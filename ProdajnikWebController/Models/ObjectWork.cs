using System;
using System.Collections.Generic;

namespace ProdajnikWebController.Models;

public partial class ObjectWork
{
    public int ObjectWorkId { get; set; }

    public int ObjectId { get; set; }

    public int WorkTypeId { get; set; }

    public virtual ConstructionObject Object { get; set; } = null!;

    public virtual WorkType WorkType { get; set; } = null!;
}
