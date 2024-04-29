using System;
using System.Collections.Generic;

namespace MovieManagementSystem.Models.Data;

public partial class Movieinfo
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Rated { get; set; }

    public string? Description { get; set; }

    public string? Poster { get; set; }

    public int? Duration { get; set; }

    public double? Price { get; set; }

    public DateOnly? LastDay { get; set; }

    public sbyte? IsActive { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
