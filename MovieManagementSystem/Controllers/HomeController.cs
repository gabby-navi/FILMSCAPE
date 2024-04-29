using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagementSystem.DataAccess.Data;
using MovieManagementSystem.Models;
using MovieManagementSystem.Models.Data;
using System.Diagnostics;

namespace MovieManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataToolDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DataToolDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _db.Movieinfos.Include(m => m.Showtimes).ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> GetMovie(int id)
        {
            try
            {
                var movie = await _db.Movieinfos.FindAsync(id);
                if (movie == null)
                {
                    return NotFound();
                }

                return RedirectToAction("BuyTickets", new { id = movie.MovieId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the movie details.");
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> BuyTickets(int id)
        {
            try
            {
                var movie = await _db.Movieinfos.Include(m => m.Showtimes).FirstOrDefaultAsync(m => m.MovieId == id);
                if (movie == null)
                {
                    return NotFound();
                }

                return View(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving movie details for editing.");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult GetOccupiedSeats(string showtime)
        {
            try
            {
                int showtimeId = int.Parse(showtime);

                var occupiedSeats = _db.Seats
                                       .Where(s => s.Showtime.ShowtimeId == showtimeId && s.TicketId != null)
                                       .Select(s => s.SeatName)
                                       .ToList();

                return Ok(occupiedSeats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching occupied seats.");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult BuyTickets([FromBody] TicketPurchaseModel purchaseData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newTicket = new Ticket
                {
                    PurchaseDate = DateTime.Now,
                    TotalCost = purchaseData.TotalCost,
                    IsPwd = purchaseData.IsPwd,
                    IsStudent = purchaseData.IsStudent
                };

                _db.Tickets.Add(newTicket);
                _db.SaveChanges();

                foreach (var seatName in purchaseData.SelectedSeats)
                {
                    var newSeat = new Seat
                    {
                        ShowtimeId = purchaseData.ShowtimeId,
                        SeatName = seatName,
                        TicketId = newTicket.TicketId
                    };

                    _db.Seats.Add(newSeat);
                }

                _db.SaveChanges();

                return Json(new { success = true, message = "Tickets purchased successfully!", ticketId = newTicket.TicketId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while purchasing tickets.");
                return StatusCode(500, "An error occurred while purchasing tickets.");
            }
        }

        public IActionResult TicketDetails(int ticketId)
        {
            var ticket = _db.Tickets
                .Include(t => t.Seats)
                    .ThenInclude(s => s.Showtime)
                        .ThenInclude(st => st.Movie)
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null || ticket.Seats == null || ticket.Seats.Count == 0)
            {
                return NotFound();
            }

            var showtimeId = ticket.Seats.FirstOrDefault().ShowtimeId;

            var showtime = _db.Showtimes
                .Include(st => st.Movie)
                .FirstOrDefault(st => st.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                return NotFound();
            }

            return View(new Tuple<Ticket, Showtime>(ticket, showtime));
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}