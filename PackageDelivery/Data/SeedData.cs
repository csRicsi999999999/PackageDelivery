using PackageDelivery.Models;

namespace PackageDelivery.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext db)
        {
            // 1. Ügyfelek feltöltése, ha még üres a tábla
            if (!db.Customers.Any())
            {
                db.Customers.AddRange(
                    new Customer { Name = "Kovács Anna", Email = "anna@example.test", PhoneNumber = "+361111111" },
                    new Customer { Name = "Nagy Péter", Email = "peter@example.test", PhoneNumber = "+362222222" }
                );
                db.SaveChanges(); // El kell menteni, hogy legyenük ID-k a csomagokhoz!
            }

            // 2. Csomagok feltöltése, ha még üres a tábla
            if (!db.Packages.Any())
            {
                // Lekérjük az előbbi ügyfeleket, hogy megadjuk a feladót és címzettet
                var anna = db.Customers.First(c => c.Name == "Kovács Anna");
                var peter = db.Customers.First(c => c.Name == "Nagy Péter");

                db.Packages.AddRange(
                    new Package
                    {
                        TrackingNumber = "PKG-2026-00125",
                        Weight = 4.5,
                        CreatedAt = DateTime.Now,
                        Status = PackageStatus.Registered,
                        SenderId = anna.Id,
                        RecipientId = peter.Id,
                        PostalCode = 1011,
                        City = "Budapest",
                        StreetName = "Fő",
                        StreetType = "utca",
                        HouseNumber = "1"
                    }
                );
                db.SaveChanges();
            }
        }
    }
}