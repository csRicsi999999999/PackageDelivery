using Microsoft.EntityFrameworkCore;
using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Net.Security;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PackageDelivery.Service
{
    public class PackageService
    {
        private readonly AppDbContext _db;
        public PackageService(AppDbContext db)
        {
            _db = db;
        }

        Package? package;
        Trackings? trackings;

        public List<Package> GetAllPackages()
        {
            return _db.Packages.ToList();
        }

        public void DisplayText(Package package)
        {

            var senders = _db.Customers.Where(s => s.Id == package.SenderId).ToList();
            var recipient = _db.Customers.Where(r => r.Id == package.RecipientId).ToList();

            Console.WriteLine($"Csomag: {package.TrackingNumber}");
            Console.WriteLine($"Feladó: {senders[0].Name}");
            Console.WriteLine($"Címzett: {recipient[0].Name}");
            Console.WriteLine($"Tömeg: {package.Weight} kg");
            Console.WriteLine($"Kézbesítési Cím: {package.PostalCode} {package.City} {package.StreetName} {package.StreetType} {package.HouseNumber}");
            Console.WriteLine($"Feladás dátuma: {package.CreatedAt}");
            Console.WriteLine($"Aktuális állapota: {package.Status}");
            Console.WriteLine("\n");
        }

        public void AddNewPackage(CustomerService customerService)
        {
            Console.Clear();
            Console.WriteLine("ÚJ CSOMAG FELADÁSA");

            var customers = customerService?.GetAllCustomers();
            bool isValid = false;

            if (customers?.Count < 2)
            {
                Console.WriteLine("\nHiba: Legalább 2 ügyfél szükséges (feladó és címzett) a csomagküldéshez!");
                Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nElérhető ügyfelek:");
            for (int i = 0; i < customers?.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i].Name} ({customers[i].Email})");
            }

            Console.Write("\nAdd meg a feladó sorszámát: ");

            int senderIndex;
            Customer? sender = null;

            while (!isValid)
            {
                try
                {
                    if (!int.TryParse(Console.ReadLine(), out senderIndex))
                    {
                        throw new ArgumentException("Számot adj meg");
                    }
                    if (senderIndex < 1 || senderIndex > customers?.Count)
                    {
                        throw new ArgumentException("Érvénytelen választás");
                    }

                    sender = customers?[senderIndex - 1];

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a számot újból:");
                }
            }

            isValid = false;

            Console.Write("Add meg a címzett sorszámát: ");

            int recipientIndex;

            while (!int.TryParse(Console.ReadLine(), out recipientIndex) || recipientIndex < 1 || recipientIndex > customers?.Count)
            {
                Console.WriteLine("Érvénytelen választás!");
                Console.WriteLine("Add meg újból a számot: ");
            }
            var recipient = customers?[recipientIndex - 1];


            if (sender?.Id == recipient?.Id)
            {
                throw new ArgumentException("A feladó és a címzett nem lehet ugyanaz a személy!");
            }

            Console.WriteLine("\n--- Kézbesítési cím megadása ---");
            Console.WriteLine("Irányítószám: ");

            int postalCode;

            while (!int.TryParse(Console.ReadLine(), out postalCode) || postalCode < 1000 || postalCode > 9999)
            {
                Console.WriteLine("Hiba: Pontosan 4 számjegyű irányítószámot adj meg!");
                Console.WriteLine("Irányítószám újra: ");

            }

            Console.Write("Település: ");
            string? city = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(city) || int.TryParse(city, out _))
            {
                Console.WriteLine("A település mezőt kötelező kitölteni.");
                Console.WriteLine("\nAdd meg újra: ");
                city = Console.ReadLine()!;
            }


            Console.Write("Közterület neve: ");
            string? street = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(street) || int.TryParse(street, out _))
            {
                Console.WriteLine("A közterület neve mezőt kötelező kitölteni.");
                Console.WriteLine("\nAdd meg újra: ");
                street = Console.ReadLine()!;
            }

            Console.Write("Közterület jellege (pl. utca, tér): ");
            string? streetType = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(streetType) || int.TryParse(streetType, out _))
            {
                Console.WriteLine("A közterület jellege mezőt kötelező kitölteni.");
                Console.WriteLine("\nAdd meg újra: ");
                streetType = Console.ReadLine()!;
            }

            Console.Write("Házszám: ");
            string? houseNumber = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(houseNumber))
            {
                Console.WriteLine("A házszám mezőt kötelező kitölteni.");
                Console.WriteLine("\nAdd meg újra: ");
                houseNumber = Console.ReadLine()!;
            }

            Console.Write("Opcionális további címadat: ");
            string? otherInfo = Console.ReadLine();

            Console.WriteLine("\n--- Csomag adatok ---");
            Console.WriteLine("1 --> Normál csomag");
            Console.WriteLine("2 --> Törékeny csomag");
            Console.WriteLine("3 --> Hűtést igénylő csomag");

            int boxType;

            Console.WriteLine("Add meg a csomag típusát: ");

            while (!int.TryParse(Console.ReadLine(), out boxType) || boxType < 1 || boxType > 3)
            {
                Console.WriteLine("Hiba: Érvénytelen csomag típus! 1, 2 vagy 3 adható meg.");
                Console.WriteLine("Add meg a csomag típusát újból: ");
            }

            Package.Type type = boxType switch
            {
                1 => Package.Type.Normal,
                2 => Package.Type.Fragile,
                3 => Package.Type.Refrigerated,
                _ => throw new ArgumentException("Érvénytelen csomag típus! 1, 2 vagy 3 adható meg.")
            };


            double weight;

            if (type == Package.Type.Normal)
            {
                Console.Write("Add meg a csomag súlyát: ");

                while (!double.TryParse(Console.ReadLine(), out weight) || weight < 0 || weight >= 30)
                {
                    Console.WriteLine("Hiba: Érvénytelen csomag súly");
                    Console.WriteLine("Add meg újra a csomag súlyát: ");
                }

                package = new Package
                {
                    SenderId = sender!.Id,
                    RecipientId = recipient!.Id,
                    PostalCode = postalCode,
                    City = city,
                    StreetName = street,
                    StreetType = streetType,
                    HouseNumber = houseNumber,
                    Weight = weight,
                    OtherInfo = otherInfo,
                    CreatedAt = DateTime.Now,
                    Status = PackageStatus.Registered,
                    BoxType = type
                };
            }

            if (type == Package.Type.Fragile)
            {
                Console.Write("Add meg a csomag súlyát: ");

                while (!double.TryParse(Console.ReadLine(), out weight) || weight < 0 || weight >= 10)
                {
                    Console.WriteLine("Hiba: Érvénytelen csomag súly");
                    Console.WriteLine("Add meg újra a csomag súlyát: ");
                }

                int fragileLevel;

                FragilePackage.Level level = 0;

                Console.WriteLine("Add meg a csomaghoz tartozó törékenységi szintet (1-3): ");
                Console.WriteLine("1 --> Alacsony, 2 --> Közepes, 3 --> Magas");
                while (!isValid)
                {
                    try
                    {
                        if (!int.TryParse(Console.ReadLine(), out fragileLevel))
                        {
                            Console.WriteLine("Hiba: Kérlek számot adj meg!");
                            Console.WriteLine("Add meg a törékenységi szintet újból: ");
                            continue;
                        }

                        level = fragileLevel switch
                        {
                            1 => FragilePackage.Level.Low,
                            2 => FragilePackage.Level.Medium,
                            3 => FragilePackage.Level.High,
                            _ => throw new ArgumentOutOfRangeException("Érvénytelen törékenységi szint! 1, 2 vagy 3 adható meg.")
                        };
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a törékenységi szintet újból.\n");
                    }
                }

                Console.WriteLine("Add meg a csomaghoz tartozó kezelési utasítást: ");
                string instructions = Console.ReadLine()!;

                package = new FragilePackage
                {
                    SenderId = sender!.Id,
                    RecipientId = recipient!.Id,
                    PostalCode = postalCode,
                    City = city,
                    StreetName = street,
                    StreetType = streetType,
                    HouseNumber = houseNumber,
                    Weight = weight,
                    FragileLevel = level,
                    HandlingInstructions = instructions,
                    OtherInfo = otherInfo,
                    CreatedAt = DateTime.Now,
                    Status = PackageStatus.Registered,
                    BoxType = type
                };
            }

            if (type == Package.Type.Refrigerated)
            {
                Console.Write("Add meg a csomag súlyát: ");

                while (!double.TryParse(Console.ReadLine(), out weight) || weight < 0 || weight >= 20)
                {
                    Console.WriteLine("Hiba: Érvénytelen csomag súly");
                    Console.WriteLine("Add meg újra a csomag súlyát: ");
                }

                Console.WriteLine("Add meg a csomag minimális tárolási hőmérsékletét Celsius-fokban: ");

                int min;

                while (!int.TryParse(Console.ReadLine(), out min) || min < -30 || min > 20)
                {
                    Console.WriteLine("Hiba: A minimális hőmérsékletnek -30 és +20 °C között kell lennie, " +
                        "és kisebbnek kell lennie a maximális értéknél!");
                    Console.WriteLine("Add meg a csomag minimális tárolási hőmérsékletét újból: ");
                }

                int max;

                while (!int.TryParse(Console.ReadLine(), out max) || max < -30 || max > 20 || max < min)
                {
                    Console.WriteLine("Hiba: A maximális hőmérsékletnek -30 és +20 °C között kell lennie, " +
                        "és nagyobbnak kell lennie a minimális értéknél!");
                    Console.WriteLine("Add meg a csomag maximális tárolási hőmérsékletét újból: ");
                }

                package = new RefrigeratedPackage
                {
                    SenderId = sender!.Id,
                    RecipientId = recipient!.Id,
                    PostalCode = postalCode,
                    City = city,
                    StreetName = street,
                    StreetType = streetType,
                    HouseNumber = houseNumber,
                    Weight = weight,
                    MinTemperature = min,
                    MaxTemperature = max,
                    OtherInfo = otherInfo,
                    CreatedAt = DateTime.Now,
                    Status = PackageStatus.Registered,
                    BoxType = type
                };
            }



            _db.Packages.Add(package!);
            _db.SaveChanges();

            foreach (var pkg in _db.Packages)
            {
                if (pkg.Id == package!.Id)
                {
                    package.Id = pkg.Id;
                    package.TrackingNumber = $"PKG-{pkg.CreatedAt.Year}-{pkg.Id.ToString("D5")}";
                    _db.SaveChanges();
                }
            }

            trackings = new Trackings
            {
                PackageId = package!.Id,
                Status = PackageStatus.Registered,
                Timestamp = DateTime.Now,
                Comment = null,
                FailureReason = null
            };

            _db.Trackings.Add(trackings!);
            _db.SaveChanges();

            Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
            Console.ReadKey();
        }
        public void PackageProcessor()
        {
            Console.WriteLine("Kérlek add meg a csomag azonosítóját: ");
            string? chooseBox = Console.ReadLine();

            string? failed = null;

            var currentPackage = _db.Packages.FirstOrDefault(p => p.TrackingNumber == chooseBox);

            if (currentPackage == null)
            {
                Console.WriteLine("Nincs ilyen csomag.");
                return;
            }

            var count = _db.Trackings
                            .Where(p => p.PackageId == currentPackage!.Id)
                            .Count(t => t.Status == PackageStatus.DeliveryFailed);

            foreach (var item in _db.Packages)
            {
                if (chooseBox == item.TrackingNumber)
                {
                    if (item.Status == PackageStatus.Returned)
                    {
                        Console.WriteLine("A csomag vissza lett küldve már a feladónak.");
                    }
                    else if (item.Status == PackageStatus.Delivered)
                    {
                        Console.WriteLine("A csomag már kézbesitve lett.");
                    }
                    else
                    {
                        int choice;

                        if (item.Status == PackageStatus.Registered)
                        {
                            Console.WriteLine($"Jelenlegi állapot: {item.Status}");
                            Console.WriteLine("Következő lehetséges állapot: 1 - Csomag elfogadva");

                            while (!int.TryParse(Console.ReadLine(), out choice) && choice != 1)
                            {
                                Console.WriteLine("Érvénytelen választás");
                                Console.WriteLine("Add meg a számot újból");
                            }

                            if (choice == 1)
                            {
                                item.Status = PackageStatus.Accepted;
                                Console.WriteLine("A csomag fogadva lett a rendszerben.");
                            }



                            else if (item.Status == PackageStatus.Accepted)
                            {
                                Console.WriteLine($"Jelenlegi állapot: {item.Status}");
                                Console.WriteLine("Következő lehetséges állapot: 1 - Kiszállítás megkezdése");

                                while (!int.TryParse(Console.ReadLine(), out choice) && choice != 1)
                                {
                                    Console.WriteLine("Érvénytelen választás");
                                    Console.WriteLine("Add meg a számot újból");
                                }
                                if (choice == 1)
                                {
                                    item.Status = PackageStatus.AtDepot;
                                    Console.WriteLine("A csomag át lett helyezve a depóba.");
                                }
                            }





                            else if (item.Status == PackageStatus.AtDepot)
                            {
                                Console.WriteLine($"Jelenlegi állapot: {item.Status}");
                                Console.WriteLine("Következő lehetséges állapot: 1 - A csomag kiszállításra kijelölve");

                                while (!int.TryParse(Console.ReadLine(), out choice) && choice != 1)
                                {
                                    Console.WriteLine("Érvénytelen választás");
                                    Console.WriteLine("Add meg a számot újból");
                                }


                                if (choice == 1)
                                {
                                    item.Status = PackageStatus.OutForDelivery;
                                    Console.WriteLine("Kiszállítás alatt.");
                                }

                            }

                            else if (item.Status == PackageStatus.OutForDelivery)
                            {
                                Console.WriteLine("Mi történt a csomaggal? (1 - Kézbesítve, 2 - Kézbesítés sikertelen)");

                                while (!int.TryParse(Console.ReadLine(), out choice) && choice != 1 && choice != 2)
                                {
                                    Console.WriteLine("Érvénytelen választás");
                                    Console.WriteLine("Add meg a számot újból");
                                }

                                int reason;

                                switch (choice)
                                {
                                    case 1:
                                        item.Status = PackageStatus.Delivered;
                                        Console.WriteLine("A csomag kézbesítve.");
                                        break;

                                    case 2:
                                        item.Status = PackageStatus.DeliveryFailed;
                                        Console.WriteLine("A csomag kézbesítése sikertelen volt, oka:");
                                        Console.WriteLine("1 - A címzett nem volt elérhető");
                                        Console.WriteLine("2 - A cím nem volt megtalálható");
                                        Console.WriteLine("3 - A címzett megtagadta az átvételt");

                                        while (!int.TryParse(Console.ReadLine(), out reason) && choice != 1 && choice != 2
                                            && choice != 3)
                                        {
                                            Console.WriteLine("Érvénytelen választás");
                                            Console.WriteLine("Add meg a számot újból");
                                        }

                                        if (reason == 1)
                                        {
                                            failed = "RecipientNotAvailable";
                                        }
                                        if (reason == 2)
                                        {
                                            failed = "AddressNotFound";
                                        }
                                        if (reason == 3)
                                        {
                                            failed = "RecipientRefused";
                                        }
                                        break;
                                }
                            }



                            else if (item.Status == PackageStatus.DeliveryFailed)
                            {
                                if (count <= 2)
                                {
                                    Console.WriteLine("Mi történt a csomaggal? (1 - Kiszállítás megkezdése, 2 - Visszaküldés)");

                                    while (!int.TryParse(Console.ReadLine(), out choice) && choice != 1 && choice != 2)
                                    {
                                        Console.WriteLine("Érvénytelen választás");
                                        Console.WriteLine("Add meg a számot újból");
                                    }

                                    switch (choice)
                                    {
                                        case 1:
                                            item.Status = PackageStatus.OutForDelivery;
                                            Console.WriteLine("A csomag kiszállításra kijelölve.");
                                            break;

                                        case 2:
                                            item.Status = PackageStatus.Returning;
                                            Console.WriteLine("A csomag visszaküldésre kerül.");
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("A csomag kézbesítése sikertelen volt már 3 alkalommal, ezért visszaküldésre kerül a feladónak.");
                                    item.Status = PackageStatus.Returning;
                                }
                            }





                            else if (item.Status == PackageStatus.Returning)
                            {
                                Console.WriteLine($"Jelenlegi állapot: {item.Status}");
                                Console.WriteLine("Következő lehetséges állapot: 1 - Visszaküldés");

                                while (!int.TryParse(Console.ReadLine(), out choice) && choice != 1)
                                {
                                    Console.WriteLine("Érvénytelen választás");
                                    Console.WriteLine("Add meg a számot újból");
                                }


                                if (choice == 1)
                                {
                                    item.Status = PackageStatus.Returned;
                                    Console.WriteLine("Visszaküldve.");
                                }
                            }

                            else if (item.Status == PackageStatus.Returned)
                            {
                                Console.WriteLine("A csomag visszaküldve a feladónak.");
                            }
                            else if (item.Status == PackageStatus.Delivered)
                            {
                                Console.WriteLine("A csomag kézbesítve.");
                            }


                            trackings = new Trackings
                            {
                                PackageId = item.Id,
                                Status = item.Status,
                                Timestamp = DateTime.Now,
                                Comment = null,
                                FailureReason = failed
                            };

                            _db.Trackings.Add(trackings!);
                            _db.SaveChanges();

                            Console.WriteLine("\n" + count);
                        }
                    }
                }
            }
        }

        public void TrackingHistory()
        {
            Console.WriteLine("Kérlek add meg a csomag azonosítóját: ");
            string? chooseBox = Console.ReadLine();
            foreach (var item in _db.Packages)
            {

                var senders = _db.Customers.Where(s => s.Id == item.SenderId).ToList();
                var recipient = _db.Customers.Where(r => r.Id == item.RecipientId).ToList();

                if (chooseBox == item.TrackingNumber)
                {
                    Console.WriteLine($"Csomag: {item.TrackingNumber}");
                    Console.WriteLine($"Feladó: {senders[0].Name}");
                    Console.WriteLine($"Címzett: {recipient[0].Name}");
                    Console.WriteLine($"Tömeg: {item.Weight} kg");
                    Console.WriteLine($"Tipus: {item.BoxType}");

                    if (item.BoxType == Package.Type.Fragile)
                    {
                        Console.WriteLine($"Törékenység: {((FragilePackage)item).FragileLevel.ToString()}");
                        Console.WriteLine($"Kezelési utasítás: {((FragilePackage)item).HandlingInstructions}");
                    }
                    if (item.BoxType == Package.Type.Refrigerated)
                    {
                        Console.WriteLine($"Megengedett tárolási hőmérséklet tartomány: {((RefrigeratedPackage)item).MinTemperature} °C és {((RefrigeratedPackage)item).MaxTemperature} °C");
                    }


                    Console.WriteLine($"Kézbesítési Cím: {item.PostalCode} {item.City} {item.StreetName} {item.StreetType} {item.HouseNumber}");
                    Console.WriteLine($"Feladás dátuma: {item.CreatedAt}");
                    Console.WriteLine($"Aktuális állapota: {item.Status}");



                    var trackingHistory = _db.Trackings.Where(t => t.PackageId == item.Id).OrderBy(t => t.Timestamp).ToList();
                    if (trackingHistory.Count == 0)
                    {
                        Console.WriteLine("Nincs nyomkövetési előzmény.");
                    }
                    else
                    {
                        Console.WriteLine("\nNyomkövetés:");
                        foreach (var tracking in trackingHistory)
                        {
                            if (tracking.FailureReason != null)
                            {
                                Console.WriteLine($"{tracking.Timestamp} {tracking.Status} - {tracking.FailureReason}");
                            }
                            else
                            {
                                Console.WriteLine($"{tracking.Timestamp} {tracking.Status}");
                            }
                        }
                    }
                }
            }
        }
    }
}
