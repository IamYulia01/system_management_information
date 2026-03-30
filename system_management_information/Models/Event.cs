using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class Event
{
    public int IdEvent { get; set; }

    public string TypeEvent { get; set; } = null!;

    public string NameEvent { get; set; } = null!;

    public string StreetEvent { get; set; } = null!;

    public string? HouseEvent { get; set; }

    public DateOnly? DateEvent { get; set; }

    public TimeOnly? TimeBeginningEvent { get; set; }

    public string? AgeLimit { get; set; }

    public virtual ICollection<RouteEventSight> RouteEventSights { get; set; } = new List<RouteEventSight>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
