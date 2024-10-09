using System;
using System.Collections.Generic;

namespace StroitelPodryadchik.Models;

public partial class ViewConstructionObjectsCustomer
{
    public int ObjectId { get; set; }

    public string НаименованиеОбъекта { get; set; } = null!;

    public string Заказчик { get; set; } = null!;

    public string ГородЗаказчика { get; set; } = null!;

    public string АдресЗаказчика { get; set; } = null!;

    public string ТелефонЗаказчика { get; set; } = null!;

    public string Генподрядчик { get; set; } = null!;

    public DateOnly ДатаЗаключенияДоговора { get; set; }

    public string? ПереченьВыполняемыхРабот { get; set; }

    public DateOnly? ДатаСдачиОбъекта { get; set; }

    public DateOnly? ДатаВводаВЭксплуатацию { get; set; }
}
