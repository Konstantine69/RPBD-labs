using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace lab6.Models;

public partial class BuildingMaterial
{
    public int MaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    public string Manufacturer { get; set; } = null!;

    public decimal PurchaseVolume { get; set; }

    public string CertificateNumber { get; set; } = null!;

    public DateOnly CertificateDate { get; set; }

    public string? Photo { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<ObjectMaterial> ObjectMaterials { get; set; } = new List<ObjectMaterial>();
}
