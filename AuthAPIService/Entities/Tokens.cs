using System;
using System.Collections.Generic;

namespace Auth.Entities
{
    public partial class Tokens
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public int Revoked { get; set; }

        public DateTime IssuedAt { get; set; }
    }
}
