using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class OperatingMode
{
    public int IdOperatingMode { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<SightOperatingMode> SightOperatingModes { get; set; } = new List<SightOperatingMode>();
}
