using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class RouteEventSight
{
    public int IdRouteEventSight { get; set; }

    public int IdRoute { get; set; }

    public int? IdEvent { get; set; }

    public int? IdSight { get; set; }

    public virtual Event? IdEventNavigation { get; set; }

    public virtual Route IdRouteNavigation { get; set; } = null!;

    public virtual Sight? IdSightNavigation { get; set; }
}
