using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Venue> Venue { get; set; }
    public DbSet<Event> Event { get; set; }
    public DbSet<Booking> Booking { get; set; }

    // Add the ConsolidatedBookingView as a DbSet
    public DbSet<ConsolidatedBookingView> ConsolidatedBookingView { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConsolidatedBookingView>().HasNoKey().ToView("ConsolidatedBookingView");
        base.OnModelCreating(modelBuilder);
    }


}
