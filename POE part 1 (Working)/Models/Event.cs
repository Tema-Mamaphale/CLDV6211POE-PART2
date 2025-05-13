using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEaseBookingSystem.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(50, ErrorMessage = "Event name cannot exceed 50 characters.")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required.")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Venue selection is required.")]
        public int VenueId { get; set; }

        public virtual Venue Venue { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
