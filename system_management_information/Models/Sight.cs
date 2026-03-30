using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class Sight
{
    public int IdSight { get; set; }

    public string TypeSight { get; set; } = null!;

    public string NameSight { get; set; } = null!;

    public string LocationStreet { get; set; } = null!;

    public string? LocationHouse { get; set; }

    public string? Description { get; set; }

    public string? ContactNumber { get; set; }

    public string? Email { get; set; }

    public string? SightUrl { get; set; }

    public virtual ICollection<PhotoSight> PhotoSights { get; set; } = new List<PhotoSight>();

    public virtual ICollection<RouteEventSight> RouteEventSights { get; set; } = new List<RouteEventSight>();

    public virtual ICollection<SightOperatingMode> SightOperatingModes { get; set; } = new List<SightOperatingMode>();
}
