using System;
using System.Collections.Generic;

namespace StroitelPodryadchik.Models;

public partial class ViewConstructionObjectsMaterial
{
    public int ObjectId { get; set; }

    public string НаименованиеОбъекта { get; set; } = null!;

    public string НаименованиеМатериала { get; set; } = null!;

    public string Производитель { get; set; } = null!;

    public decimal ОбъемЗакупки { get; set; }

    public string НомерСертификата { get; set; } = null!;

    public DateOnly ДатаСертификата { get; set; }
}
