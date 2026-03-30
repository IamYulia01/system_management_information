using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class SpecialDaySight
{
    public int IdSpecialDaySight { get; set; }

    public string? SpecialDayStatus { get; set; }

    public DateOnly SpecialDayDate { get; set; }

    public virtual ICollection<SightOperatingMode> SightOperatingModes { get; set; } = new List<SightOperatingMode>();
}
