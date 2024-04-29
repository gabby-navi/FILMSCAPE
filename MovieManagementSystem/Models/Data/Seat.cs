using System;
using System.Collections.Generic;

namespace MovieManagementSystem.Models.Data;

public partial class Seat
{
    public int SeatId { get; set; }

    public string SeatName { get; set; } = null!;

    public int ShowtimeId { get; set; }

    public int TicketId { get; set; }

    public virtual Showtime Showtime { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
