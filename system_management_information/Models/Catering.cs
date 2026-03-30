using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class Catering
{
    public int IdCatering { get; set; }

    public string EstablishmentCategory { get; set; } = null!;

    public string EstablishmentName { get; set; } = null!;

    public string EstablishmentStreet { get; set; } = null!;

    public string EstablishmentHouse { get; set; } = null!;

    public string? EstablishmentPhone { get; set; }

    public string? CateringUrl { get; set; }

    public virtual ICollection<CateringModeOperationCatering> CateringModeOperationCaterings { get; set; } = new List<CateringModeOperationCatering>();

    public virtual ICollection<CateringTypeKitchen> CateringTypeKitchens { get; set; } = new List<CateringTypeKitchen>();

    public virtual ICollection<RouteCateringHotel> RouteCateringHotels { get; set; } = new List<RouteCateringHotel>();
}
