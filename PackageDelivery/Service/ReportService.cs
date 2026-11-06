using Microsoft.EntityFrameworkCore.Migrations.Operations;
using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace PackageDelivery.Service
{
    public class ReportService
    {
        private readonly AppDbContext _db;
        public ReportService(AppDbContext db)
        {
            _db = db;
        }
        public void NumberOfPackagesByStatus()
        {
            foreach (var item in _db.Packages
                .Where(p => p.Status >= 0)
                .OrderByDescending(p => p.TrackingNumber)
                .GroupBy(p => p.Status))
            {
                Console.WriteLine("\n" + item.Key + ": " + item.Count() + "db ilyen állapotú csomag van");
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void PackageTypeStatistics()
        {

            foreach (var item in _db.Packages
                        .OrderByDescending(p => p.BoxType)
                        .GroupBy(p => p.BoxType))
            { 
                Console.WriteLine($"\nTípus: {item.Key} - {item.Count()} db ilyen típusú csomag van");
                Console.WriteLine($"Összes súly: {item.Sum(p => p.Weight):F2} kg");
                Console.WriteLine($"Átlagos súly: {item.Average(p => p.Weight):F2} kg");
            }

            Console.WriteLine("Nyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void TopFiveSenders()
        { 
            var packages = _db.Packages.ToList();

            var item = packages
                .GroupBy(p => p.SenderId)
                .OrderByDescending(p => p.Count())
                .Take(5);

            foreach (var sender in item)
            {
                var customer = _db.Customers.FirstOrDefault(c => c.Id == sender.Key);

                Console.WriteLine($"\nFeladó: {customer?.Name} - {sender.Count()} db csomagot adott fel, összes súly: {sender.Sum(p => p.Weight)} kg");
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void ReasonsForUnsuccessfulDeliveries()
        {
            var packages = _db.Trackings.ToList();

            var item = packages
                .Where(p => p.FailureReason != null)
                .GroupBy(p => p.FailureReason)
                .OrderByDescending(p => p.Count());

            foreach (var reason in item)
            {
                Console.WriteLine($"\nHiba oka: {reason.Key} - {reason.Count()} db sikertelen kézbesítés");
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void TimePeriodReport()
        {
            DateTime startDate = DateTime.MinValue;
            bool isValidStartDate = false;

            while (!isValidStartDate)
            {
                Console.WriteLine("Add meg a kezdő dátumot (YYYY-MM-DD): ");
                string input = Console.ReadLine()!;

                if (DateTime.TryParse(input, out startDate))
                {
                     isValidStartDate = true;
                }
                else
                {
                    Console.WriteLine("Hibás dátumformátum! Kérlek add meg újra.");
                }
            }

            DateTime endDate = DateTime.MaxValue;
            bool isValidEndDate = false;

            while (!isValidEndDate)
            {
                Console.WriteLine("Add meg a záró dátumot (YYYY-MM-DD): ");
                string input = Console.ReadLine()!;

                if (DateTime.TryParse(input, out endDate))
                {
                    isValidEndDate = true;
                }
                else
                {
                    Console.WriteLine("Hibás dátumformátum! Kérlek add meg újra.");
                }
            }

            endDate = endDate.Date.AddDays(1).AddTicks(-1);

            var packages = _db.Packages
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .OrderBy(p => p.CreatedAt)
                .ToList();

            Console.WriteLine($"\n{packages.Count} db csomag lett feladva a megadott időszakban.");
            Console.WriteLine($"Összes súly: {packages.Sum(p => p.Weight)} kg");
            Console.WriteLine($"Átlagos súly: {packages.Average(p => p.Weight)} kg");

            Console.WriteLine("\n");

            foreach (var item in packages)
            {
                Console.WriteLine($"Csomagszám: {item.TrackingNumber} - Feladási idő: {item.CreatedAt}");
            }
            
            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void SpecialHandlingPackages()
        {
            var packages = _db.Packages
                .Where(p => p.BoxType == Package.Type.Fragile || p.BoxType == Package.Type.Refrigerated)
                .Where(p => p.Status != PackageStatus.Delivered && p.Status != PackageStatus.Returned)
                .OrderBy(p => p.CreatedAt)
                .ToList();

            if (packages.Count == 0)
            {
                Console.WriteLine("\nNincs speciális kezelést igénylő csomag.");
                Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
                Console.ReadKey();
                return;
            }

            foreach (var item in packages)
            {
                if (item.BoxType == Package.Type.Fragile)
                {
                    Console.WriteLine($"Tipus: {item.BoxType}");
                    Console.WriteLine("Törékenységi szint: " + ((FragilePackage)item).FragileLevel);
                    Console.WriteLine("Kezelési utasítások: " + ((FragilePackage)item).HandlingInstructions);
                    Console.WriteLine("\n");
                }
                else if (item.BoxType == Package.Type.Refrigerated)
                {
                    Console.WriteLine($"Tipus: {item.BoxType}");
                    Console.WriteLine($"Hőmérséklet: {((RefrigeratedPackage)item).MinTemperature} °C és {((RefrigeratedPackage)item).MaxTemperature} °C");
                }
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }
    }
}
