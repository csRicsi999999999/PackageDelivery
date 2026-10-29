using Microsoft.EntityFrameworkCore;
using PackageDelivery.Data;
using PackageDelivery.Service;
using PackageDelivery.UI;

namespace PackageDelivery;

internal class Program
{
    static void Main(string[] args)
    {
        bool running = true;

        using AppDbContext db = new();
        db.Database.Migrate();
        SeedData.Initialize(db);

        var customerService = new CustomerService(db);
        var packageService = new PackageService(db);

        var menu = new ConsoleMenu(customerService, packageService);
        

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("Csomagküldő szolgáltatás kezelő rendszere");
            Console.WriteLine();
            Console.WriteLine("1. Ügyfelek kezelése");
            Console.WriteLine("2. Új csomag feladása");
            Console.WriteLine("3. Csomag feldolgozása");
            Console.WriteLine("4. Csomag nyomkövetése");
            Console.WriteLine("5. Csomagok keresése");
            Console.WriteLine("6. Riportok");
            Console.WriteLine("0. Kilépés");
            Console.WriteLine();

            Console.Write("Választás: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // TODO: Itt hívd meg az ügyfelek kezeléséért felelős saját komponensedet.
                    menu.HandleCustomerMenu();
                    break;

                case "2":
                    // TODO: Itt hívd meg az új csomag feladásáért felelős saját komponensedet.
                    packageService.AddNewPackage(customerService);
                    break;

                case "3":
                    // TODO: Itt hívd meg a csomag feldolgozásáért és állapotkezeléséért felelős saját komponensedet.
                    packageService.PackageProcessor();
                    break;

                case "4":
                    // TODO: Itt hívd meg a csomag nyomkövetéséért felelős saját komponensedet.
                    Console.WriteLine("Ez a funkció még nincs megvalósítva.");
                    break;

                case "5":
                    // TODO: Itt hívd meg a csomagkeresésekért felelős saját komponensedet.
                    Console.WriteLine("Ez a funkció még nincs megvalósítva.");
                    break;

                case "6":
                    // TODO: Itt hívd meg a riportok elkészítéséért felelős saját komponensedet.
                    Console.WriteLine("Ez a funkció még nincs megvalósítva.");
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Érvénytelen választás.");
                    break;
            }
        }
    }
}
