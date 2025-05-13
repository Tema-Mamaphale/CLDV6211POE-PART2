using EventEaseBookingSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class VenueController : Controller
{
    private readonly AppDbContext _context;

    public VenueController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Venue
    public async Task<IActionResult> Index()
    {
        return View(await _context.Venue.ToListAsync());
    }

    // GET: Venue/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            TempData["Error"] = "Invalid venue ID.";
            return RedirectToAction(nameof(Index));
        }

        var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
        if (venue == null)
        {
            TempData["Error"] = "Venue not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(venue);
    }

    // GET: Venue/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Venue/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "All fields are required.";
            return View(venue);
        }

        try
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                venue.ImageUrl = await UploadImageToBlobAsync(imageFile);
            }

            _context.Add(venue);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Venue created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred: {ex.Message}";
            return View(venue);
        }
    }

    // GET: Venue/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            TempData["Error"] = "Invalid venue ID.";
            return RedirectToAction(nameof(Index));
        }

        var venue = await _context.Venue.FindAsync(id);
        if (venue == null)
        {
            TempData["Error"] = "Venue not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(venue);
    }

    // POST: Venue/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Venue venue, IFormFile imageFile)
    {
        if (id != venue.VenueId)
        {
            TempData["Error"] = "Venue ID mismatch.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "All fields are required.";
            return View(venue);
        }

        try
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                venue.ImageUrl = await UploadImageToBlobAsync(imageFile);
            }

            _context.Update(venue);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Venue updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VenueExists(venue.VenueId))
            {
                TempData["Error"] = "Venue not found.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                throw;
            }
        }
    }

    // GET: Venue/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            TempData["Error"] = "Invalid venue ID.";
            return RedirectToAction(nameof(Index));
        }

        var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
        if (venue == null)
        {
            TempData["Error"] = "Venue not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(venue);
    }

    // POST: Venue/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var hasBookings = await _context.Booking.AnyAsync(b => b.VenueId == id);
            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete the venue as it is linked to existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                TempData["Error"] = "Venue not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Venue deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    // Helper method to upload an image to Azure Blob Storage
    private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
    {
        string connectionString = "cloud";
        string containerName = "cldv6211images";

        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        string blobName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        using (var stream = imageFile.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        return blobClient.Uri.ToString();
    }

    // Helper method to check if a venue exists
    private bool VenueExists(int id)
    {
        return _context.Venue.Any(e => e.VenueId == id);
    }
}
