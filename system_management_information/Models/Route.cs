using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class Route
{
    public int IdRoute { get; set; }

    public DateOnly DateCreation { get; set; }

    public string? NameRoute { get; set; }

    public long IdUser { get; set; }

    public virtual Userbot IdUserNavigation { get; set; } = null!;

    public virtual ICollection<RouteCateringHotel> RouteCateringHotels { get; set; } = new List<RouteCateringHotel>();

    public virtual ICollection<RouteEventSight> RouteEventSights { get; set; } = new List<RouteEventSight>();
}
