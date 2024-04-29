using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagementSystem.DataAccess.Data;
using MovieManagementSystem.Models.Data;

namespace MovieManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataToolDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public AdminController(DataToolDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<IActionResult> Admin()
        {
            var movies = await _db.Movieinfos.ToListAsync();
            return View(movies);
        }
        
        public IActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(Movieinfo model, string[] showtimes, IFormFile poster)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (poster != null && poster.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(poster.FileName);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await poster.CopyToAsync(stream);
                    }

                    model.Poster = "/uploads/" + fileName;
                }

                _db.Movieinfos.Add(model);
                await _db.SaveChangesAsync();

                foreach (var time in showtimes)
                {
                    model.Showtimes.Add(new Showtime { Showtime1 = time });
                }

                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Movie created successfully!" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while adding the movie. Please try again.");
                return View(model);
            }
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

                return RedirectToAction("EditMovie", new { id = movie.MovieId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the movie details.");
                return RedirectToAction("Admin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditMovie(int id)
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
                return StatusCode(500, new { success = false, message = "An error occurred while creating movie." });
            }
        }

        [HttpPost]
        private async Task UploadPoster(IFormFile poster, Movieinfo movie)
        {
            if (poster != null)
            {
                if (!string.IsNullOrEmpty(movie.Poster))
                {
                    var existingPosterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", movie.Poster.TrimStart('/'));
                    if (System.IO.File.Exists(existingPosterPath))
                    {
                        System.IO.File.Delete(existingPosterPath);
                    }
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(poster.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await poster.CopyToAsync(stream);
                }

                movie.Poster = "/uploads/" + fileName;
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditMovie(int movieid, IFormFile poster, Movieinfo updatedMovie)
        {
            try
            {
                var movie = await _db.Movieinfos.FindAsync(movieid);
                if (movie == null)
                {
                    return NotFound();
                }

                movie.Title = updatedMovie.Title;
                movie.Description = updatedMovie.Description;
                movie.Price = updatedMovie.Price;
                movie.LastDay = updatedMovie.LastDay;
                movie.IsActive = updatedMovie.IsActive;

                if (poster != null)
                {
                    await UploadPoster(poster, movie);
                }

                _db.Entry(movie).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Movie's details have been updated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating movie details.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating movie details" });
            }
        }

        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var movie = await _db.Movieinfos.FindAsync(id);
                if (movie == null)
                {
                    return NotFound();
                }

                var showtimeIds = _db.Showtimes.Where(s => s.MovieId == id).Select(s => s.ShowtimeId).ToList();

                foreach (var showtimeId in showtimeIds)
                {
                    var seats = _db.Seats.Where(seat => seat.ShowtimeId == showtimeId);
                    _db.Seats.RemoveRange(seats);
                }

                _db.Showtimes.RemoveRange(_db.Showtimes.Where(s => s.MovieId == id));

                _db.Movieinfos.Remove(movie);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Movie deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting movie" });
            }
        }
    }
}
