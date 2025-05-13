using System;
using System.ComponentModel.DataAnnotations;

namespace EventEaseBookingSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int VenueId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        public virtual Event? Event { get; set; }
        public virtual Venue? Venue { get; set; }
    }
}
