using System;
using System.Collections.Generic;
using System.Text;

namespace PackageDelivery.Models
{
    public class RefrigeratedPackage : Package
    {
        public double? MinTemperature { get; set; }
        public double? MaxTemperature { get; set; }
    }
}
