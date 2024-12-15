using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace lab6.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string OrganizationName { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<ConstructionObject> ConstructionObjects { get; set; } = new List<ConstructionObject>();
}
