using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class TypeKitchen
{
    public int IdTypeKitchen { get; set; }

    public string NameTypeKitchen { get; set; } = null!;

    public virtual ICollection<CateringTypeKitchen> CateringTypeKitchens { get; set; } = new List<CateringTypeKitchen>();
}
