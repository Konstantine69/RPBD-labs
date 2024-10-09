using System;
using System.Collections.Generic;

namespace StroitelPodryadchik.Models;

public partial class ViewConstructionObjectsWorkType
{
    public int ObjectId { get; set; }

    public string НаименованиеОбъекта { get; set; } = null!;

    public string НомерЛицензии { get; set; } = null!;

    public DateOnly ДатаЛицензии { get; set; }

    public DateOnly СрокДействияЛицензии { get; set; }

    public string КодВКлассификаторе { get; set; } = null!;
}
