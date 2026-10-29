using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PackageDelivery.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string? TrackingNumber { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; } 
        public Customer? Sender { get; set; }
        public Customer? Recipient { get; set; }
        public double Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostalCode { get; set; }
        public string? City { get; set; }
        public string? StreetName { get; set; }
        public string? StreetType { get; set; }
        public string? HouseNumber { get; set; }
        public string? OtherInfo { get; set; }
        public PackageStatus Status { get; set; } = PackageStatus.Registered;
        public int BoxType { get; set;} 

    }
}
