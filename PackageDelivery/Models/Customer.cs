using System;
using System.Collections.Generic;
using System.Text;

namespace PackageDelivery.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Package> SentPackages { get; set; } = new();
        public List<Package> RecipientPackages { get; set; } = new();

    }
}
