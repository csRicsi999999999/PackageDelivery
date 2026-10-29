using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
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

        public void AddNewPackage(CustomerService customerService)
        {
            Console.Clear();
            Console.WriteLine("ÚJ CSOMAG FELADÁSA");

            var customers = customerService?.GetAllCustomers();

            if (customers?.Count < 2)
            {
                Console.WriteLine("\nHiba: Legalább 2 ügyfél szükséges (feladó és címzett) a csomagküldéshez!");
                Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
                Console.ReadKey();
            }

            Console.WriteLine("\nElérhető ügyfelek:");
            for (int i = 0; i < customers?.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i].Name} ({customers[i].Email})");
            }

            Console.Write("\nAdd meg a feladó sorszámát: ");
            if (!int.TryParse(Console.ReadLine(), out int senderIndex) || senderIndex < 1 || senderIndex > customers?.Count)
            {
                Console.WriteLine("Érvénytelen választás!");
                Console.ReadKey();
            }
            var sender = customers?[senderIndex - 1];

            Console.Write("Add meg a címzett sorszámát: ");
            if (!int.TryParse(Console.ReadLine(), out int recipientIndex) || recipientIndex < 1 || recipientIndex > customers?.Count)
            {
                Console.WriteLine("Érvénytelen választás!");
                Console.ReadKey();
            }
            var recipient = customers?[recipientIndex - 1];

            if (sender?.Id == recipient?.Id)
            {
                Console.WriteLine("Hiba: A feladó és a címzett nem lehet ugyanaz az személy!");
                Console.ReadKey();
            }

            Console.WriteLine("\n--- Kézbesítési cím megadása ---");
            Console.Write("Irányítószám: ");
            int postalCode = int.Parse(Console.ReadLine()!);

            Console.Write("Település: ");
            string? city = Console.ReadLine();

            Console.Write("Közterület neve: ");
            string? street = Console.ReadLine();

            Console.Write("Közterület jellege (pl. utca, tér): ");
            string? streetType = Console.ReadLine();

            Console.Write("Házszám: ");
            string? houseNumber = Console.ReadLine();

            Console.Write("Opcionális további címadat: ");
            string? otherInfo = Console.ReadLine();

            Console.WriteLine("\n--- Csomag adatok ---");
            Console.WriteLine("1 --> Normál csomag");
            Console.WriteLine("2 --> Törékeny csomag");
            Console.WriteLine("3 --> Hűtést igénylő csomag");

            Console.WriteLine("Add meg a csomag típusát: ");
            int boxType = int.Parse(Console.ReadLine()!);

            double weight = 0;

            if (boxType == 1)
            {
                Console.Write("Add meg a csomag súlyát: ");
                weight = double.Parse(Console.ReadLine()!);
                if (weight < 0 || weight > 30)
                {
                    Console.WriteLine("Hiba: A súlynak 0 és 30 kg között kell lennie!");
                    Console.WriteLine("Add meg a csomag súlyát újból: ");
                    weight = double.Parse(Console.ReadLine()!);
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
                    Status = PackageStatus.Registered
                };
            }

            if (boxType == 2)
            {
                Console.Write("Add meg a csomag súlyát: ");
                weight = double.Parse(Console.ReadLine()!);
                if (weight < 0 || weight > 10)
                {
                    Console.WriteLine("Hiba: A súlynak 0 és 10 kg között kell lennie!");
                    Console.WriteLine("Add meg a csomag súlyát újból: ");
                    weight = double.Parse(Console.ReadLine()!);
                }
                Console.WriteLine("Add meg a csomaghoz tartozó törékenységi szintet (1-3): ");
                Console.WriteLine("1 --> Alacsony, 2 --> Közepes, 3 --> Magas");
                int fragileLevel = int.Parse(Console.ReadLine()!);

                FragilePackage.Level level = fragileLevel switch
                {
                    1 => FragilePackage.Level.Low,
                    2 => FragilePackage.Level.Medium,
                    3 => FragilePackage.Level.High,
                    _ => throw new ArgumentException("Érvénytelen törékenységi szint! 1, 2 vagy 3 adható meg.")
                };

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
                    Status = PackageStatus.Registered
                };
            }

            if (boxType == 3)
            {
                Console.Write("Add meg a csomag súlyát: ");
                weight = double.Parse(Console.ReadLine()!);
                if (weight < 0 || weight > 20)
                {
                    Console.WriteLine("Hiba: A súlynak 0 és 20 kg között kell lennie!");
                    Console.WriteLine("Add meg a csomag súlyát újból: ");
                    weight = double.Parse(Console.ReadLine()!);
                }

                Console.WriteLine("Add meg a csomag minimális tárolási hőmérsékletét Celsius-fokban: ");
                int min = int.Parse(Console.ReadLine()!);
                Console.WriteLine("Add meg a csomag maximális tárolási hőmérsékletét Celsius-fokban: ");
                int max = int.Parse(Console.ReadLine()!);

                if (min < -30 || min > 20 || min > max)
                {
                    Console.WriteLine("Hiba: A minimális hőmérsékletnek -30 és +20 °C között kell lennie, " +
                        "és kisebbnek kell lennie a maximális értéknél!");
                    Console.WriteLine("Add meg a csomag minimális tárolási hőmérsékletét újból: ");
                    min = int.Parse(Console.ReadLine()!);
                }

                if (max < -30 || max > 20 || max < min)
                {
                    Console.WriteLine("Hiba: A maximális hőmérsékletnek -30 és +20 °C között kell lennie, " +
                        "és nagyobbnak kell lennie a minimális értéknél!");
                    Console.WriteLine("Add meg a csomag maximális tárolási hőmérsékletét újból: ");
                    max = int.Parse(Console.ReadLine()!);
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
                Comment = "A csomag regisztrálva lett a rendszerben.",
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

            foreach(var item in _db.Packages)
            {
                if (chooseBox == item.TrackingNumber)
                {
                    Console.WriteLine(item.Status);
                    //test
                }
            }
        }
    }
}
