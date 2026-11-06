using Microsoft.Extensions.DependencyModel.Resolution;
using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PackageDelivery.Service
{
    public class SearchService
    {
        private readonly AppDbContext _db;
        private readonly PackageService _packageService;
        public SearchService(AppDbContext db)
        {
            _db = db;
            _packageService = new PackageService(_db);
        }
        bool isValid = false;
        public void SearchByTrackingId()
        {
            Console.WriteLine("Adj meg kérlek egy nyomkövetési azonosítót: ");

            string? trackingId = null;

            while (!isValid)
            {
                trackingId = Console.ReadLine();
                Package? currentPackage = _db.Packages.FirstOrDefault(p => p.TrackingNumber == trackingId);
                try
                {
                    if (string.IsNullOrWhiteSpace(trackingId))
                    {
                        throw new ArgumentException("Ez egy kötelező mező");
                    }
                    if (int.TryParse(trackingId, out _))
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
            foreach (var item in _db.Packages.
                Where(p => p.TrackingNumber == trackingId))
            {
                _packageService.DisplayText(item);
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void SearchBySender(CustomerService customerService)
        {
            var customers = customerService?.GetAllCustomers();

            Console.WriteLine("\nElérhető ügyfelek:");
            for (int i = 0; i < customers?.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i].Name} ({customers[i].Email})");
            }

            Console.Write("\nAdd meg a feladó sorszámát: ");

            int senderIndex;
            Customer? sender = null;
            isValid = false;

            while (!isValid)
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
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a számot újból:");
                }

            foreach (var item in _db.Packages
                .Where(p => p.SenderId == sender!.Id)
                .OrderByDescending(p => p.CreatedAt))
            {
                _packageService.DisplayText(item);
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void SearchByRecipient(CustomerService customerService)
        {
            var customers = customerService?.GetAllCustomers();


            Console.WriteLine("\nElérhető ügyfelek:");
            for (int i = 0; i < customers?.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i].Name} ({customers[i].Email})");
            }

            Console.Write("\nAdd meg a címzett sorszámát: ");

            int recipientIndex;
            Customer? recipient = null;
            isValid = false;

            while (!isValid)
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
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a számot újból:");
                }

            foreach (var item in _db.Packages
                .Where(p => p.RecipientId == recipient!.Id)
                .OrderByDescending(p => p.CreatedAt))
            {
                _packageService.DisplayText(item);
            }

        }

        private void DisplayPackagesByStatus(PackageStatus status)
        {
            var packages = _db.Packages.Where(p => p.Status == status).ToList();

            foreach (var item in packages)
            {
                _packageService.DisplayText(item);
            }
        }
        public void SearchByStatus()
        {
            isValid = false;

            while (!isValid)
            {
                try
                {
                    Console.WriteLine("Választható státuszok: ");
                    Console.WriteLine("1 - Feladva ");
                    Console.WriteLine("2 - Átvéve ");
                    Console.WriteLine("3 - Elosztóközpontban ");
                    Console.WriteLine("4 - Kiszállítás alatt ");
                    Console.WriteLine("5 - Sikertelen kézbesítés ");
                    Console.WriteLine("6 - Visszaküldés alatt ");
                    Console.WriteLine("7 - Visszaküldve ");
                    Console.WriteLine("8 - Kézbesítve ");
                    Console.WriteLine("Válassz ki egy státuszt: ");
                    string? choose = Console.ReadLine();


                    switch (choose)
                    {
                        case "1":
                            DisplayPackagesByStatus(PackageStatus.Registered);
                            isValid = true;
                            break;

                        case "2":
                            DisplayPackagesByStatus(PackageStatus.Accepted);
                            isValid = true;
                            break;

                        case "3":
                            DisplayPackagesByStatus(PackageStatus.AtDepot);
                            isValid = true;
                            break;

                        case "4":
                            DisplayPackagesByStatus(PackageStatus.OutForDelivery);
                            isValid = true;
                            break;

                        case "5":
                            DisplayPackagesByStatus(PackageStatus.DeliveryFailed);
                            isValid = true;
                            break;

                        case "6":
                            DisplayPackagesByStatus(PackageStatus.Returning);
                            isValid = true;
                            break;


                        case "7":
                            DisplayPackagesByStatus(PackageStatus.Returned);
                            isValid = true;
                            break;

                        case "8":
                            DisplayPackagesByStatus(PackageStatus.Delivered);
                            isValid = true;
                            break;

                        default:
                            throw new ArgumentException("Érvénytelen választás");

                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a számot újból:");
                }
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();

        }

        private void DisplayPackagesByType(Package.Type boxType)
        {
            var packages = _db.Packages.Where(p => p.BoxType == boxType).ToList();

            foreach (var item in packages)
            {
                _packageService.DisplayText(item);
            }
        }
        public void SearchByPackageType()
        {
            isValid = false;

            while (!isValid)
            {
                try
                {
                    Console.WriteLine(" Választható típusú csomagok ");
                    Console.WriteLine("1 - Normál csomag ");
                    Console.WriteLine("2 - Törékeny csomag ");
                    Console.WriteLine("3 - Hűtést igénylő csomag ");
                    Console.WriteLine("Válassz ki egy típust: ");
                    string? choose = Console.ReadLine();


                    switch (choose)
                    {
                        case "1":
                            DisplayPackagesByType(Package.Type.Normal);
                            isValid = true;
                            break;

                        case "2":
                            DisplayPackagesByType(Package.Type.Fragile);
                            isValid = true;
                            break;

                        case "3":
                            DisplayPackagesByType(Package.Type.Refrigerated);
                            isValid = true;
                            break;

                        default:
                            throw new ArgumentException("Érvénytelen választás");
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Add meg a számot újból:");
                }

            }
            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void SearchByPostingPeriod()
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

            foreach (var item in _db.Packages
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .OrderBy(p => p.CreatedAt))
            {
                _packageService.DisplayText(item);
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();

        }
    }
}
