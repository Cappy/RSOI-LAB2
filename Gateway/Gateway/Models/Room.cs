using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Models
{
    public class Room
    {
        public Guid RoomId { get; set; }
        public int Number { get; set; }
        public double Cost { get; set; }
    }
}
