using System;
using System.Collections.Generic;
using System.Text;

namespace PackageDelivery.Models
{
    public class Trackings
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Comment { get; set; } 
        public string? FailureReason { get; set; }
        public PackageStatus Status { get; set; }
        public Package? Package { get; set; }
    }
}
