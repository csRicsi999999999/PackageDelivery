using Microsoft.EntityFrameworkCore.Migrations.Operations;
using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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
            var packages = _db.Packages
                .Where(p => p.Status >= 0)
                .GroupBy(p => p.Status)
                .OrderByDescending(p => p.Count())
                .ToList();

            if (packages.Count == 0)
            {
                Console.WriteLine("Nincs a feltételnek megfelelő csomag.");
            }

            else
            {
                foreach (var item in packages)
                {
                    Console.WriteLine("\n" + item.Key + ": " + item.Count() + "db ilyen állapotú csomag van");
                }
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void PackageTypeStatistics()
        {

            var packages = _db.Packages
                        .GroupBy(p => p.BoxType)
                        .Select(g => new
                        {
                            BoxType = g.Key,
                            Count = g.Count(),
                            TotalWeight = g.Sum(p => p.Weight),
                            AvgWeight = g.Average(p => p.Weight)
                        })
                        .ToList();

            var result = Enum.GetValues<Package.Type>()
                        .Select(type =>
                        {
                            var match = packages.FirstOrDefault(g => g.BoxType == type);
                            return new
                        {
                            BoxType = type,
                            Count = match?.Count ?? 0,
                            TotalWeight = match?.TotalWeight ?? 0.0,
                            AvgWeight = match?.AvgWeight ?? 0.0
                        };
                        })
                        .OrderByDescending(x => x.Count)
                        .ToList();

            foreach (var item in result)
            {
                Console.WriteLine($"\nTípus: {item.BoxType} - {item.Count} db ilyen típusú csomag van");
                Console.WriteLine($"Összes súly: {item.TotalWeight:F2} kg");
                Console.WriteLine($"Átlagos súly: {item.AvgWeight:F2} kg");
            }

            Console.WriteLine("Nyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void TopFiveSenders()
        {

            var packages = _db.Packages
                .GroupBy(p => p.SenderId)
                .Select(g => new
                {
                    SenderId = g.Key,
                    Count = g.Count(),
                    TotalWeight = g.Sum(p => p.Weight)
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            if (packages.Count == 0)
            {
                Console.WriteLine("\nNincs a feltételnek megfelelő csomag.");
                Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
                Console.ReadKey();
                return;
            }

            var senderIds = packages.Select(x => x.SenderId).ToList();
            var senderNames = _db.Customers
                .Where(c => senderIds.Contains(c.Id))
                .ToDictionary(c => c.Id, c => c.Name);

            if (packages.Count < 5)
            {
                Console.WriteLine("\nNincs még elegendő feladott csomag a TOP5-ös statisztikához.");
            }

            Console.WriteLine($"TOP {packages.Count}: ");

            foreach (var item in packages)
            {
                string? senderName = senderNames.TryGetValue(item.SenderId, out var name)
                    ? name
                    : "Ismeretlen ügyfél";

                Console.WriteLine($"\nFeladó: {senderName} - {item.Count} db csomagot adott fel, összes súly: {item.TotalWeight:F2} kg");
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();

        }

        public void ReasonsForUnsuccessfulDeliveries()
        {

            string[] knownReasons = { "RecipientNotAvailable", "AddressNotFound", "RecipientRefused" };

            var packages = _db.Trackings
                .Where(p => p.FailureReason != null)
                .GroupBy(p => p.FailureReason)
                .Select(g => new
                {
                    Reason = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var result = knownReasons
                .Select(r => new
                {
                    Reason = r,
                    Count = packages.FirstOrDefault(g => g.Reason == r)?.Count ?? 0
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            foreach (var item in result)
            {
                Console.WriteLine($"\nHiba oka: {item.Reason} - {item.Count} db sikertelen kézbesítés");
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
                    if (startDate > endDate)
                    {
                        Console.WriteLine("A kezdő dátumnak kisebbnek vagy egyenlőnek kell lennie, mint a záró dátumnak!");
                    }
                    else
                    {
                        isValidEndDate = true;
                    }
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

            if (packages.Count == 0)
            {
                Console.WriteLine("Nincs a feltételnek megfelelő csomag.");
            }

            else
            {
                double totalWeight = packages
                    .Sum(p => p.Weight);

                double avgWeight = packages
                    .Select(p => p.Weight)
                    .DefaultIfEmpty(0)
                    .Average();


                Console.WriteLine($"\n{packages.Count} db csomag lett feladva a megadott időszakban.");
                Console.WriteLine($"Összes súly: {totalWeight:F2} kg");
                Console.WriteLine($"Átlagos súly: {avgWeight:F2} kg");

                Console.WriteLine("\n");

                foreach (var item in packages)
                {
                    Console.WriteLine($"Csomagszám: {item.TrackingNumber} - Feladási idő: {item.CreatedAt}");
                }
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
            }

            else
            {
                foreach (var item in packages)
                {
                    if (item.BoxType == Package.Type.Fragile)
                    {
                        Console.WriteLine($"Csomag: {item.TrackingNumber}");
                        Console.WriteLine($"Tipus: {item.BoxType}");
                        Console.WriteLine("Törékenységi szint: " + ((FragilePackage)item).FragileLevel);
                        Console.WriteLine("Kezelési utasítások: " + ((FragilePackage)item).HandlingInstructions);
                        Console.WriteLine("\n");
                    }
                    else if (item.BoxType == Package.Type.Refrigerated)
                    {
                        Console.WriteLine($"Csomag: {item.TrackingNumber}");
                        Console.WriteLine($"Tipus: {item.BoxType}");
                        Console.WriteLine($"Hőmérséklet: {((RefrigeratedPackage)item).MinTemperature} °C és {((RefrigeratedPackage)item).MaxTemperature} °C");
                        Console.WriteLine("\n");
                    }
                }
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }
    }
}
