using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class SightOperatingMode
{
    public int IdSightOperatingMode { get; set; }

    public int IdSight { get; set; }

    public int? IdOperatingMode { get; set; }

    public int? WorkingDayWeek { get; set; }

    public int? IdSpecialDaySight { get; set; }

    public virtual OperatingMode? IdOperatingModeNavigation { get; set; }

    public virtual Sight IdSightNavigation { get; set; } = null!;

    public virtual SpecialDaySight? IdSpecialDaySightNavigation { get; set; }
}
