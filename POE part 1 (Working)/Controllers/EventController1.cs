using EventEaseBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class EventController : Controller
{
    private readonly AppDbContext _context;

    public EventController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Event
    public async Task<IActionResult> Index()
    {
        var events = await _context.Event.Include(e => e.Venue).ToListAsync();
        return View(events);
    }

    // GET: Event/Create
    public IActionResult Create()
    {
        ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName");
        return View();
    }

    // POST: Event/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventId,EventName,EventDate,VenueId")] Event eventItem)
    {
        try
        {
            // Check for duplicate event on the same date at the same venue
            bool eventExists = await _context.Event.AnyAsync(e =>
                e.EventName == eventItem.EventName &&
                e.EventDate == eventItem.EventDate &&
                e.VenueId == eventItem.VenueId);

            if (eventExists)
            {
                TempData["ErrorMessage"] = "An event with the same name already exists on the selected date at this venue.";
                return View(eventItem);
            }

            _context.Add(eventItem);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "An error occurred while saving the event. Please try again later.";
            return View(eventItem);
        }
    }

    // GET: Event/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return RedirectToAction(nameof(Index));

        var eventItem = await _context.Event.FindAsync(id);
        if (eventItem == null) return RedirectToAction(nameof(Index));

        ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", eventItem.VenueId);
        return View(eventItem);
    }

    // POST: Event/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,VenueId")] Event eventItem)
    {
        if (id != eventItem.EventId) return RedirectToAction(nameof(Index));

        try
        {
            _context.Update(eventItem);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "An error occurred while updating the event. Please try again.";
            return View(eventItem);
        }
    }

    // GET: Event/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return RedirectToAction(nameof(Index));

        var eventItem = await _context.Event
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(m => m.EventId == id);

        if (eventItem == null) return RedirectToAction(nameof(Index));

        return View(eventItem);
    }

    // POST: Event/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var eventItem = await _context.Event.FindAsync(id);
            if (eventItem != null)
            {
                bool hasBookings = await _context.Booking.AnyAsync(b => b.EventId == id);
                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Cannot delete the event as it is linked to existing bookings.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Event.Remove(eventItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Event not found.";
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An unexpected error occurred while deleting the event.";
        }

        return RedirectToAction(nameof(Index));
    }
}
