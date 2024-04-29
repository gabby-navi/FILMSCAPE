using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManagementSystem.Models.Data;

public partial class Ticket
{
    public int TicketId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int TotalCost { get; set; }

    public sbyte? IsPwd { get; set; }

    public sbyte? IsStudent { get; set; }

    public int? TicketNum { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}

public class TicketPurchaseModel
{
    public string? MovieTitle { get; set; }

    [Required]
    public int ShowtimeId { get; set; }

    public List<string>? SelectedSeats { get; set; }
    public int Quantity { get; set; }
    public sbyte? IsPwd { get; set; }
    public sbyte? IsStudent { get; set; }
    public int TotalCost { get; set; }
}


