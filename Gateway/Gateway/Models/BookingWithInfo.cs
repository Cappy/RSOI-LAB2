using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Models
{
    public class BookingWithInfo
    {
        public Guid BookingId { get; set; }
        public Customer customer { get; set; }
        public Room room { get; set; }
    }
}
