using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class Hotel
{
    public int IdHotel { get; set; }

    public string HotelName { get; set; } = null!;

    public string HotelStreet { get; set; } = null!;

    public string HotelHouse { get; set; } = null!;

    public string ContactNumberHotel { get; set; } = null!;

    public string? HotelUrl { get; set; }

    public string? TypeHotel { get; set; }

    public virtual ICollection<PhotoHotel> PhotoHotels { get; set; } = new List<PhotoHotel>();

    public virtual ICollection<RouteCateringHotel> RouteCateringHotels { get; set; } = new List<RouteCateringHotel>();
}
