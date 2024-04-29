using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MovieManagementSystem.Models.Data;

namespace MovieManagementSystem.DataAccess.Data;

public partial class DataToolDbContext : DbContext
{
    public DataToolDbContext(DbContextOptions<DataToolDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Movieinfo> Movieinfos { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Movieinfo>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PRIMARY");

            entity.ToTable("movieinfo");

            entity.Property(e => e.MovieId)
                .HasColumnType("int(4)")
                .HasColumnName("movieId");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.Duration)
                .HasColumnType("int(11)")
                .HasColumnName("duration");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("isActive");
            entity.Property(e => e.LastDay).HasColumnName("lastDay");
            entity.Property(e => e.Poster)
                .HasMaxLength(50)
                .HasColumnName("poster");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Rated)
                .HasMaxLength(50)
                .HasColumnName("rated");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PRIMARY");

            entity.ToTable("seats");

            entity.HasIndex(e => e.ShowtimeId, "FK_seats_showtimes");

            entity.HasIndex(e => e.TicketId, "FK_seats_tickets");

            entity.Property(e => e.SeatId)
                .HasColumnType("int(11)")
                .HasColumnName("seatId");
            entity.Property(e => e.SeatName)
                .HasMaxLength(50)
                .HasColumnName("seatName");
            entity.Property(e => e.ShowtimeId)
                .HasColumnType("int(5)")
                .HasColumnName("showtimeId");
            entity.Property(e => e.TicketId)
                .HasColumnType("int(11)")
                .HasColumnName("ticketId");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Seats)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_seats_showtimes");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Seats)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_seats_tickets");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.ShowtimeId).HasName("PRIMARY");

            entity.ToTable("showtimes");

            entity.HasIndex(e => e.MovieId, "FK_showtimes_movieinfo");

            entity.Property(e => e.ShowtimeId)
                .HasColumnType("int(5)")
                .HasColumnName("showtimeId");
            entity.Property(e => e.MovieId)
                .HasColumnType("int(5)")
                .HasColumnName("movieId");
            entity.Property(e => e.Showtime1)
                .HasMaxLength(50)
                .HasColumnName("showtime");
            entity.Property(e => e.TotalSeats)
                .HasDefaultValueSql("'50'")
                .HasColumnType("int(11)")
                .HasColumnName("totalSeats");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_showtimes_movieinfo");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PRIMARY");

            entity.ToTable("tickets");

            entity.Property(e => e.TicketId)
                .HasColumnType("int(11)")
                .HasColumnName("ticketId");
            entity.Property(e => e.IsPwd)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("isPWD");
            entity.Property(e => e.IsStudent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("isStudent");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("datetime")
                .HasColumnName("purchaseDate");
            entity.Property(e => e.TicketNum)
                .HasColumnType("int(11)")
                .HasColumnName("ticketNum");
            entity.Property(e => e.TotalCost)
                .HasColumnType("int(11)")
                .HasColumnName("totalCost");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
