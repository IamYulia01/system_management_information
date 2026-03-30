using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class SpecialDayCatering
{
    public int IdSpecialDayCatering { get; set; }

    public string? StatusDay { get; set; }

    public DateOnly Date { get; set; }

    public virtual ICollection<CateringModeOperationCatering> CateringModeOperationCaterings { get; set; } = new List<CateringModeOperationCatering>();
}
