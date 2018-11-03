using System;
using System.Collections.Generic;

namespace CustomersAPIServices.Models
{
    public partial class Customers
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int PhoneNumber { get; set; }
    }
}
