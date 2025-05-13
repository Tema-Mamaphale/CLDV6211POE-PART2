using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EventEaseBookingSystem.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required]
        [StringLength(50)]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public int Capacity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
