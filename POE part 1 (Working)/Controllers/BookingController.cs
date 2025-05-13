using EventEaseBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class BookingController : Controller
{
    private readonly AppDbContext _context;

    public BookingController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Booking
    public async Task<IActionResult> Index(string searchString)
    {
        var bookings = _context.ConsolidatedBookingView.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            bookings = bookings.Where(b =>
                b.BookingId.ToString().Contains(searchString) ||
                b.EventName.Contains(searchString));
        }

        var bookingList = await bookings.ToListAsync();
        ViewData["SearchString"] = searchString;
        return View(bookingList);
    }

    // GET: Booking/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var booking = await _context.Booking
            .Include(b => b.Event)
            .Include(b => b.Venue)
            .FirstOrDefaultAsync(m => m.BookingId == id);

        if (booking == null) return NotFound();

        return View(booking);
    }

    // GET: Booking/Create
    public IActionResult Create()
    {
        ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName");
        ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName");
        return View();
    }

    // POST: Booking/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookingId,BookingDate,EventId,VenueId")] Booking booking)
    {
        booking.BookingDate = booking.BookingDate.Date;

        bool isAlreadyBooked = _context.Booking.Any(b =>
            b.VenueId == booking.VenueId &&
            b.BookingDate == booking.BookingDate);

        if (isAlreadyBooked)
        {
            ViewBag.ErrorMessage = "This venue is already booked on the selected date.";
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        try
        {
            _context.Add(booking);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ViewBag.ErrorMessage = "An error occurred while creating the booking.";
        }

        ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
        ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
        return View(booking);
    }

    // GET: Booking/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var booking = await _context.Booking.FindAsync(id);
        if (booking == null) return NotFound();

        ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
        ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
        return View(booking);
    }

    // POST: Booking/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingDate,EventId,VenueId")] Booking booking)
    {
        if (id != booking.BookingId) return NotFound();

        try
        {
            _context.Update(booking);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Booking updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ViewBag.ErrorMessage = "An error occurred while updating the booking.";
        }

        ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
        ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
        return View(booking);
    }

    // GET: Booking/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var booking = await _context.Booking
            .Include(b => b.Event)
            .Include(b => b.Venue)
            .FirstOrDefaultAsync(m => m.BookingId == id);

        if (booking == null) return NotFound();

        return View(booking);
    }

    // POST: Booking/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _context.Booking.FindAsync(id);
        if (booking == null) return NotFound();

        _context.Booking.Remove(booking);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Booking deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    private bool BookingExists(int id)
    {
        return _context.Booking.Any(e => e.BookingId == id);
    }
}
