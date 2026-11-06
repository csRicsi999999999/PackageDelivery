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
using static PackageDelivery.Models.FragilePackage;

namespace PackageDelivery.Service
{
    public class PackageService
    {
        private readonly AppDbContext _db;
        private Dictionary<PackageStatus, HashSet<PackageStatus>> _validPackageStatusChanges = new();
        public PackageService(AppDbContext db)
        {
            _db = db;

            AllowStatusChange(PackageStatus.Registered, PackageStatus.Accepted);
            AllowStatusChange(PackageStatus.Accepted, PackageStatus.AtDepot);
            AllowStatusChange(PackageStatus.AtDepot, PackageStatus.OutForDelivery);
            AllowStatusChange(PackageStatus.OutForDelivery, PackageStatus.Delivered);
            AllowStatusChange(PackageStatus.OutForDelivery, PackageStatus.DeliveryFailed);
            AllowStatusChange(PackageStatus.DeliveryFailed, PackageStatus.OutForDelivery);
            AllowStatusChange(PackageStatus.DeliveryFailed, PackageStatus.Returning);
            AllowStatusChange(PackageStatus.Returning, PackageStatus.Returned);
        }

        private void AllowStatusChange(PackageStatus current, PackageStatus next)
        {
            if (_validPackageStatusChanges.TryGetValue(current, out var options))
            {
                options.Add(next);
            }
            else
            {
                _validPackageStatusChanges.Add(current, new HashSet<PackageStatus> { next });
            }
        }

        private bool IsStateChangeValid(PackageStatus current, PackageStatus next)
        {
            if (_validPackageStatusChanges.TryGetValue(current, out var nextOptions))
            {
                return nextOptions.Contains(next);
            }
            return false;
        }

        public string ChangePackageStatus(int packageId, PackageStatus nextStatus, string? failureReason = null)
        {
            Package? pkg = _db.Packages.Find(packageId);
            if (pkg == null) return "Nincs ilyen csomag azonosító.";

            if (!IsStateChangeValid(pkg.Status, nextStatus))
            {
                return "Ez az állapot váltás nem engedélyezett.";
            }

            pkg.Status = nextStatus;


            var tracking = new Trackings
            {
                PackageId = pkg.Id,
                Status = nextStatus,
                Timestamp = DateTime.Now,
                Comment = null,
                FailureReason = failureReason
            };

            _db.Trackings.Add(tracking);
            _db.SaveChanges();

            return "Sikeres állapot változás.";
        }

        Package? package;
        Trackings? trackings;

        bool isValid = false;

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

            var customers = customerService.GetAllCustomers();

            if (customers?.Count < 2)
            {
                Console.WriteLine("\nHiba: Legalább 2 ügyfél szükséges (feladó és címzett) a csomagküldéshez!");
                Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
                Console.ReadKey();
                return;
            }

            customers = customerService.GetAllCustomers();
            Console.WriteLine("Ügyfelek listája: ");
            foreach (var c in customers)
            {
                Console.WriteLine($"{c.Id}: {c.Name}");
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

            Console.Write("Add meg a címzett sorszámát: ");

            int recipientIndex;
            Customer? recipient = null;
            isValid = false;

            while (!isValid)
            {
                try
                {
                    if (!int.TryParse(Console.ReadLine(), out recipientIndex))
                    {
                        throw new ArgumentException("Számot adj meg");
                    }
                    if (recipientIndex < 1 || recipientIndex > customers?.Count)
                    {
                        throw new ArgumentException("Érvénytelen választás");
                    }

                    recipient = customers?[recipientIndex - 1];

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a számot újból:");
                }
            }


            if (sender?.Id == recipient?.Id)
            {
                throw new ArgumentException("A feladó és a címzett nem lehet ugyanaz a személy!");
            }

            Console.WriteLine("\n--- Kézbesítési cím megadása ---");
            Console.WriteLine("Irányítószám: ");

            int postalCode = 0;
            isValid = false;

            while (!isValid)
            {
                try
                {
                    if (!int.TryParse(Console.ReadLine(), out postalCode))
                    {
                        throw new ArgumentException("Csak számjegy adható meg");
                    }
                    if (postalCode < 1000 || postalCode > 9999)
                    {
                        throw new ArgumentException("Pontosan 4 számjegyű irányítószám adható meg.!");
                    }

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg az irányítószámot újra:");
                }
            }

            Console.Write("Település: ");

            string? city = null;
            isValid = false;

            while (!isValid)
            {
                city = Console.ReadLine();
                try
                {
                    if (string.IsNullOrWhiteSpace(city))
                    {
                        throw new ArgumentException("Ez egy kötelező mező");
                    }
                    if (int.TryParse(city, out _))
                    {
                        throw new ArgumentException("Csak betűket adhatsz meg!");
                    }

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a települést újra:");
                }
            }


            Console.Write("Közterület neve: ");

            string? street = null;
            isValid = false;

            while (!isValid)
            {
                street = Console.ReadLine();
                try
                {
                    if (string.IsNullOrWhiteSpace(street))
                    {
                        throw new ArgumentException("Ez egy kötelező mező");
                    }
                    if (int.TryParse(street, out _))
                    {
                        throw new ArgumentException("Csak betűket adhatsz meg!");
                    }

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg az utcanevet újra:");
                }
            }

            Console.Write("Közterület jellege (pl. utca, tér): ");

            string? streetType = null;
            isValid = false;

            while (!isValid)
            {
                streetType = Console.ReadLine();
                try
                {
                    if (string.IsNullOrWhiteSpace(streetType))
                    {
                        throw new ArgumentException("Ez egy kötelező mező");
                    }
                    if (int.TryParse(streetType, out _))
                    {
                        throw new ArgumentException("Csak betűket adhatsz meg!");
                    }

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a közterület jellegét újra:");
                }
            }

            Console.Write("Házszám: ");

            string? houseNumber = null;
            isValid = false;

            while (!isValid)
            {
                houseNumber = Console.ReadLine();
                try
                {
                    if (string.IsNullOrWhiteSpace(houseNumber))
                    {
                        throw new ArgumentException("Ez egy kötelező mező");
                    }

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a házszámot újra:");
                }
            }

            Console.Write("Opcionális további címadat: ");
            string? otherInfo = Console.ReadLine();

            Console.WriteLine("\n--- Csomag adatok ---");
            Console.WriteLine("1 --> Normál csomag");
            Console.WriteLine("2 --> Törékeny csomag");
            Console.WriteLine("3 --> Hűtést igénylő csomag");

            int boxType;
            isValid = false;

            Package.Type type = 0;

            Console.WriteLine("Add meg a csomag típusát: ");

            while (!isValid)
            {
                try
                {
                    if (!int.TryParse(Console.ReadLine(), out boxType))
                    {
                        throw new ArgumentException("Szám adható csak meg");
                    }


                    type = boxType switch
                    {
                        1 => Package.Type.Normal,
                        2 => Package.Type.Fragile,
                        3 => Package.Type.Refrigerated,
                        _ => throw new ArgumentException("Érvénytelen csomag típus! 1, 2 vagy 3 adható meg.")
                    };

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a csomag típusát újból:");
                }
            }

            double weight = 0;
            isValid = false;

            if (type == Package.Type.Normal)
            {
                Console.Write("Add meg a csomag súlyát: ");

                while (!isValid)
                {
                    try
                    {
                        if (!double.TryParse(Console.ReadLine(), out weight))
                        {
                            throw new ArgumentException("Szám adható csak meg");
                        }
                        if (weight < 0 || weight >= 30)
                        {
                            throw new ArgumentException("A súlynak 0 és 30 között kell lennie");
                        }

                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a súlyt újból:");
                    }
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

                isValid = false;

                while (!isValid)
                {
                    try
                    {
                        if (!double.TryParse(Console.ReadLine(), out weight))
                        {
                            throw new ArgumentException("Szám adható csak meg");
                        }
                        if (weight < 0 || weight >= 10)
                        {
                            throw new ArgumentException("A súlynak 0 és 10 között kell lennie");
                        }

                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a súlyt újból:");
                    }
                }

                int fragileLevel;
                isValid = false;

                FragilePackage.Level level = 0;

                Console.WriteLine("Add meg a csomaghoz tartozó törékenységi szintet (1-3): ");
                Console.WriteLine("1 --> Alacsony, 2 --> Közepes, 3 --> Magas");
                while (!isValid)
                {
                    try
                    {
                        if (!int.TryParse(Console.ReadLine(), out fragileLevel))
                        {
                            Console.WriteLine("Szám adható csak meg!");
                            Console.WriteLine("Add meg a törékenységi szintet újból: ");
                            continue;
                        }

                        level = fragileLevel switch
                        {
                            1 => FragilePackage.Level.Low,
                            2 => FragilePackage.Level.Medium,
                            3 => FragilePackage.Level.High,
                            _ => throw new ArgumentException("Érvénytelen törékenységi szint! 1, 2 vagy 3 adható meg.")
                        };
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a törékenységi szintet újból: ");
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

                isValid = false;

                while (!isValid)
                {
                    try
                    {
                        if (!double.TryParse(Console.ReadLine(), out weight))
                        {
                            throw new ArgumentException("Szám adható csak meg");
                        }
                        if (weight < 0 || weight >= 20)
                        {
                            throw new ArgumentException("A súlynak 0 és 20 között kell lennie");
                        }

                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a súlyt újból:");
                    }
                }

                Console.WriteLine("Add meg a csomag minimális tárolási hőmérsékletét Celsius-fokban: ");

                int min = 0;
                int max = 0;
                isValid = false;

                while (!isValid)
                {
                    try
                    {
                        if (!int.TryParse(Console.ReadLine(), out min))
                        {
                            throw new ArgumentException("Szám adható csak meg");
                        }
                        if (min < -30 || min > 20 || min > max)
                        {
                            throw new ArgumentException("A hőmérsékletnek -30 és +20 °C között kell lennie");
                        }

                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a minimális hőmérsékletet újból:");
                    }
                }

                isValid = false;

                while (!isValid)
                {
                    try
                    {
                        if (!int.TryParse(Console.ReadLine(), out max))
                        {
                            throw new ArgumentException("Szám adható csak meg");
                        }
                        if (max < -30 || max > 20 || max < min)
                        {
                            throw new ArgumentException("A hőmérsékletnek -30 és +20 °C között kell lennie");
                        }

                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a maximális hőmérsékletet újból:");
                    }
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
            string? chooseBox = null;
            Package? currentPackage = null;

            isValid = false;

            while (!isValid)
            {
                chooseBox = Console.ReadLine();
                currentPackage = _db.Packages.FirstOrDefault(p => p.TrackingNumber == chooseBox);
                try
                {
                    if (string.IsNullOrWhiteSpace(chooseBox))
                    {
                        throw new ArgumentException("Ez egy kötelező mező");
                    }
                    if (int.TryParse(chooseBox, out _))
                    {
                        throw new ArgumentException("Érvénytelen csomagazonosító!");
                    }
                    if (currentPackage! == null)
                    {
                        throw new ArgumentException("Nincs ilyen csomag azonosító.");
                    }

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a csomag azonosítóját újra:");
                }
            }

            string? failed = null;

            string? choice = null;

            if (currentPackage!.Status == PackageStatus.Returned)
            {
                Console.WriteLine("A csomag vissza lett küldve már a feladónak.");
                return;
            }
            if (currentPackage!.Status == PackageStatus.Delivered)
            {
                Console.WriteLine("A csomag már kézbesitve lett.");
                return;
            }

            var count = _db.Trackings
                            .Where(p => p.PackageId == currentPackage!.Id)
                            .Count(t => t.Status == PackageStatus.DeliveryFailed);

            Console.WriteLine($"Jelenlegi állapot: {currentPackage.Status}");
            

            if (currentPackage!.Status == PackageStatus.Registered)
            {
                isValid = false;
                while (!isValid)
                {
                    try
                    {
                        Console.WriteLine("Következő lehetséges állapot: 1 - Csomag elfogadva");
                        string mode = Console.ReadLine()!;

                        _ = mode switch
                        {
                            "1" => ChangePackageStatus(currentPackage.Id, PackageStatus.Accepted),
                            _ => throw new ArgumentException("Érvénytelen állapot átmenet.")
                        };
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a számot újból!");
                    }
                }
            }
            else if (currentPackage!.Status == PackageStatus.Accepted)
            {
                isValid = false;
                while (!isValid)
                {
                    try
                    {
                        Console.WriteLine("Következő lehetséges állapot: 1 - Kiszállítás megkezdése");
                        string mode = Console.ReadLine()!;

                        _ = mode switch
                        {
                            "1" => ChangePackageStatus(currentPackage.Id, PackageStatus.AtDepot),
                            _ => throw new ArgumentException("Érvénytelen állapot átmenet.")
                        };
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a számot újból!");
                    }
                }
            }
            else if (currentPackage!.Status == PackageStatus.AtDepot)
            {
                isValid = false;
                while (!isValid)
                {
                    try
                    {
                        Console.WriteLine("Következő lehetséges állapot: 1 - A csomag kiszállításra kijelölve");
                        string mode = Console.ReadLine()!;

                        _ = mode switch
                        {
                            "1" => ChangePackageStatus(currentPackage.Id, PackageStatus.OutForDelivery),
                            _ => throw new ArgumentException("Érvénytelen állapot átmenet.")
                        };
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a számot újból!");
                    }
                }
            }
            else if (currentPackage!.Status == PackageStatus.OutForDelivery)
            {
                isValid = false;
                bool reasonValid = false;
                while (!isValid)
                {
                    try
                    {
                        Console.WriteLine("Következő lehetséges állapot: 1 - Kézbesítve, 2 - Kézbesítés sikertelen");
                        string mode = Console.ReadLine()!;

                        if (mode == "1")
                        {
                            ChangePackageStatus(currentPackage.Id, PackageStatus.Delivered);
                        }
                        else if (mode == "2")
                        {
                            while (!reasonValid)
                            {
                                try
                                {
                                    Console.WriteLine("Mi történt a csomaggal? (1 - A címzett nem volt elérhető, 2 - A cím nem volt megtalálható, 3 - A címzett megtagadta az átvételt)");
                                    choice = Console.ReadLine()!;
                                    failed = choice switch
                                    {
                                        "1" => "RecipientNotAvailable",
                                        "2" => "AddressNotFound",
                                        "3" => "RecipientRefused",
                                        _ => throw new ArgumentException("Érvénytelen választás")
                                    };
                                    reasonValid = true;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine("Add meg a számot újból!");
                                }
                            }
                            ChangePackageStatus(currentPackage.Id, PackageStatus.DeliveryFailed);
                        }
                        else
                        {
                            throw new ArgumentException("Érvénytelen állapot átmenet.");
                        }
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a számot újból!");
                    }
                }
            }
            else if (currentPackage!.Status == PackageStatus.DeliveryFailed)
            {
                if (count <= 2)
                {
                    isValid = false;
                    while (!isValid)
                    {
                        try
                        {
                            Console.WriteLine("Következő lehetséges állapot: 1 - Kiszállítás megkezdése, 2 - Visszaküldés");
                            string mode = Console.ReadLine()!;
                            _ = mode switch
                            {
                                "1" => ChangePackageStatus(currentPackage.Id, PackageStatus.OutForDelivery),
                                "2" => ChangePackageStatus(currentPackage.Id, PackageStatus.Returning),
                                _ => throw new ArgumentException("Érvénytelen állapot átmenet.")
                            };
                            isValid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("Add meg a számot újból!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("A csomag kézbesítése sikertelen volt már 3 alkalommal, ezért visszaküldésre kerül a feladónak.");
                    ChangePackageStatus(currentPackage.Id, PackageStatus.Returning);
                }
            }
            else if (currentPackage!.Status == PackageStatus.Returning)
            {
                isValid = false;

                while (!isValid)
                {
                    try
                    {
                        Console.WriteLine("Következő lehetséges állapot: 1 - Visszaküldve");
                        string mode = Console.ReadLine()!;
                        _ = mode switch
                        {
                            "1" => ChangePackageStatus(currentPackage.Id, PackageStatus.Returned),
                            _ => throw new ArgumentException("Érvénytelen állapot átmenet.")
                        };
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Add meg a számot újból!");
                    }
                }
            }
        }




        public void TrackingHistory()
        {
            Console.WriteLine("Kérlek add meg a csomag azonosítóját: ");
            string? chooseBox;
            Package? currentPackage = null;

            while (!isValid)
            {
                chooseBox = Console.ReadLine();
                currentPackage = _db.Packages.FirstOrDefault(p => p.TrackingNumber == chooseBox);
                try
                {
                    if (string.IsNullOrWhiteSpace(chooseBox))
                    {
                        throw new ArgumentException("Ez egy kötelező mező");
                    }
                    if (int.TryParse(chooseBox, out _))
                    {
                        throw new ArgumentException("Érvénytelen csomagazonosító!");
                    }
                    if (currentPackage! == null)
                    {
                        throw new ArgumentException("Nincs ilyen csomag azonosító.");
                    }

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a csomag azonosítóját újra:");
                }
            }

            var senders = _db.Customers.Where(s => s.Id == currentPackage!.SenderId).ToList();
            var recipient = _db.Customers.Where(r => r.Id == currentPackage!.RecipientId).ToList();



            Console.WriteLine($"Csomag: {currentPackage!.TrackingNumber}");
            Console.WriteLine($"Feladó: {senders[0].Name}");
            Console.WriteLine($"Címzett: {recipient[0].Name}");
            Console.WriteLine($"Tömeg: {currentPackage!.Weight} kg");
            Console.WriteLine($"Tipus: {currentPackage!.BoxType}");

            if (currentPackage!.BoxType == Package.Type.Fragile)
            {
                Console.WriteLine($"Törékenység: {((FragilePackage)currentPackage).FragileLevel.ToString()}");
                Console.WriteLine($"Kezelési utasítás: {((FragilePackage)currentPackage).HandlingInstructions}");
            }
            if (currentPackage!.BoxType == Package.Type.Refrigerated)
            {
                Console.WriteLine($"Megengedett tárolási hőmérséklet tartomány: {((RefrigeratedPackage)currentPackage).MinTemperature} °C és {((RefrigeratedPackage)currentPackage).MaxTemperature} °C");
            }


            Console.WriteLine($"Kézbesítési Cím: {currentPackage!.PostalCode} {currentPackage!.City} {currentPackage!.StreetName} {currentPackage!.StreetType} {currentPackage!.HouseNumber}");
            Console.WriteLine($"Feladás dátuma: {currentPackage!.CreatedAt}");
            Console.WriteLine($"Aktuális állapota: {currentPackage!.Status}");


            var trackingHistory = _db.Trackings.Where(t => t.PackageId == currentPackage!.Id).OrderBy(t => t.Timestamp).ToList();
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
