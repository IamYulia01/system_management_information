using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class ModeOperationCatering
{
    public int IdModeOperationCatering { get; set; }

    public TimeOnly Beginning { get; set; }

    public TimeOnly EndDay { get; set; }

    public virtual ICollection<CateringModeOperationCatering> CateringModeOperationCaterings { get; set; } = new List<CateringModeOperationCatering>();
}
