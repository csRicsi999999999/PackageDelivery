using System;
using System.Collections.Generic;
using System.Text;

namespace PackageDelivery.Models
{
    public class FragilePackage : Package
    {
        public enum Level { Low, Medium, High }
        public Level FragileLevel { get; set; }
        public string? HandlingInstructions { get; set; }
    }
}
