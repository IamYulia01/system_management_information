using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class Ticket
{
    public int IdTicket { get; set; }

    public int? MinimumAge { get; set; }

    public int? MaximumAge { get; set; }

    public decimal? Price { get; set; }

    public int IdEvent { get; set; }

    public int? CountPeople { get; set; }

    public virtual Event IdEventNavigation { get; set; } = null!;
}
