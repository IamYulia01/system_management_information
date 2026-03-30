using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class RouteCateringHotel
{
    public int IdRouteCateringHotel { get; set; }

    public int IdRoute { get; set; }

    public int? IdCatering { get; set; }

    public int? IdHotel { get; set; }

    public virtual Catering? IdCateringNavigation { get; set; }

    public virtual Hotel? IdHotelNavigation { get; set; }

    public virtual Route IdRouteNavigation { get; set; } = null!;
}
