using System;
using Microsoft.EntityFrameworkCore;

namespace EventEaseBookingSystem.Models
{
    [Keyless]
    public class ConsolidatedBookingView
    {
        public int BookingId { get; set; }
        public string EventName { get; set; }
        public string VenueName { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
