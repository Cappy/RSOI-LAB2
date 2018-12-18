using System;
using System.Collections.Generic;

namespace AuthService.Models
{
    public partial class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public enum UserRole
    {
        NORMAL,
        ADMIN
    }
}
