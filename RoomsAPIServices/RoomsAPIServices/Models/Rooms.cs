using System;
using System.Collections.Generic;

namespace RoomsAPIServices.Models
{
    public partial class Rooms
    {
        public Guid RoomId { get; set; }
        public int Number { get; set; }
        public double Cost { get; set; }
    }
}
