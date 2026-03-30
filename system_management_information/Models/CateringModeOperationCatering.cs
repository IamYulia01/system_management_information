using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class CateringModeOperationCatering
{
    public int IdCateringModeOperationCatering { get; set; }

    public int? IdModeOperationCatering { get; set; }

    public int IdCatering { get; set; }

    public int? DayWeek { get; set; }

    public int? IdSpecialDayCatering { get; set; }

    public virtual Catering IdCateringNavigation { get; set; } = null!;

    public virtual ModeOperationCatering? IdModeOperationCateringNavigation { get; set; }

    public virtual SpecialDayCatering? IdSpecialDayCateringNavigation { get; set; }
}
