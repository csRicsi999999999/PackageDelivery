using Microsoft.Extensions.DependencyModel.Resolution;
using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
        public void SearchByTrackingId()
        {
            Console.WriteLine("Adj meg kérlek egy nyomkövetési azonosítót: ");
            string? trackingId = Console.ReadLine();

            bool found = false;

            foreach (var item in _db.Packages.
                Where(p => p.TrackingNumber == trackingId))
            {
                _packageService.DisplayText(item);
            }

            if (!found)
            {
                Console.WriteLine("Nincs találat a megadott nyomkövetési azonosítóval.");
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
            if (!int.TryParse(Console.ReadLine(), out int senderIndex) || senderIndex < 1 || senderIndex > customers?.Count)
            {
                Console.WriteLine("Érvénytelen választás!");
                Console.ReadKey();
            }

            var sender = customers?[senderIndex - 1];

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
            if (!int.TryParse(Console.ReadLine(), out int recipientIndex) || recipientIndex < 1 || recipientIndex > customers?.Count)
            {
                Console.WriteLine("Érvénytelen választás!");
                Console.ReadKey();
            }

            var recipient = customers?[recipientIndex - 1];

            foreach (var item in _db.Packages
                .Where(p => p.RecipientId == recipient!.Id)
                .OrderByDescending(p => p.CreatedAt))
            {
                _packageService.DisplayText(item);
            }
            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();

        }

        public void SearchByStatus()
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
                    foreach (var item in _db.Packages
                         .Where(p => p.Status == PackageStatus.Registered))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "2":
                    foreach (var item in _db.Packages
                    .Where(p => p.Status == PackageStatus.Accepted))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "3":
                    foreach (var item in _db.Packages
                    .Where(p => p.Status == PackageStatus.AtDepot))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "4":
                    foreach (var item in _db.Packages
                    .Where(p => p.Status == PackageStatus.OutForDelivery))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "5":
                    foreach (var item in _db.Packages
                    .Where(p => p.Status == PackageStatus.DeliveryFailed))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "6":
                    foreach (var item in _db.Packages
                    .Where(p => p.Status == PackageStatus.Returning))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "7":
                    foreach (var item in _db.Packages
                    .Where(p => p.Status == PackageStatus.Returned))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "8":
                    foreach (var item in _db.Packages
                    .Where(p => p.Status == PackageStatus.Delivered))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                default:
                    Console.WriteLine("Érvénytelen választás");
                    return;
            }


            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();

        }
        public void SearchByPackageType()
        {
            Console.WriteLine("Választható típusú csomagok: ");
            Console.WriteLine("1 - Normál csomag ");
            Console.WriteLine("2 - Törékeny csomag ");
            Console.WriteLine("3 - Hűtést igénylő csomag ");

            Console.WriteLine("Válassz ki egy típust: ");
            string? choose = Console.ReadLine();


            switch (choose)
            {
                case "1":
                    foreach (var item in _db.Packages
                         .Where(p => p.BoxType == 1))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "2":
                    foreach (var item in _db.Packages
                    .Where(p => p.BoxType == 2))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                case "3":
                    foreach (var item in _db.Packages
                    .Where(p => p.BoxType == 3))
                    {
                        _packageService.DisplayText(item);
                    }
                    break;

                default:
                    Console.WriteLine("Érvénytelen választás");
                    return;
            }


            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void SearchByPostingPeriod()
        {
            Console.WriteLine("Adj meg egy kezdő dátumot: ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Hibás formátumú kezdő dátum!");
                return;
            }

            Console.WriteLine("Adj meg egy záró dátumot: ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                Console.WriteLine("Hibás formátumú záró dátum!");
                return;
            }

            if (startDate > endDate)
            {
                Console.WriteLine("A kezdő dátum nem lehet későbbi a záró dátumnál.");
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
