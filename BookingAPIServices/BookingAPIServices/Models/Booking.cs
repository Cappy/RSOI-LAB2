using System;
using System.Collections.Generic;

namespace BookingAPIServices.Models
{
    public partial class Booking
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RoomId { get; set; }
    }
}
