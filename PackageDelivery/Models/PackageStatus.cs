using System;
using System.Collections.Generic;
using System.Text;

namespace PackageDelivery.Models
{
    public enum PackageStatus
    {
        Registered, // Feladva
        Accepted, // Átvéve
        AtDepot, // Elosztóközpontban
        OutForDelivery, // Kiszállítás alatt
        DeliveryFailed, // Sikertelen kézbesítés
        Delivered, // Kézbesítve
        Returning, // Visszaküldés alatt
        Returned, // Visszaküldve 
    }
}
