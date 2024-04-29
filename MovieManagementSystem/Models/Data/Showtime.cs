using System;
using System.Collections.Generic;

namespace MovieManagementSystem.Models.Data;

public partial class Showtime
{
    public int ShowtimeId { get; set; }

    public int MovieId { get; set; }

    public string Showtime1 { get; set; } = null!;

    public int TotalSeats { get; set; }

    public virtual Movieinfo Movie { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
