﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace TechSupportXPress.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int? GenderId { get; set; }
        public string Gender { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        public bool IsDeleted { get; set; } = false;
    }
}
