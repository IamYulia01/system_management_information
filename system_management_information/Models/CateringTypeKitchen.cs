using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class CateringTypeKitchen
{
    public int IdCateringTypeKitchen { get; set; }

    public int IdCatering { get; set; }

    public int IdTypeKitchen { get; set; }

    public virtual Catering IdCateringNavigation { get; set; } = null!;

    public virtual TypeKitchen IdTypeKitchenNavigation { get; set; } = null!;
}
