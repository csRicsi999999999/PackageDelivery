using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PackageDelivery.Models;
using PackageDelivery.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PackageDelivery.UI
{
    public class ConsoleMenu
    {
        private readonly CustomerService? _customerService;
        private readonly SearchService? _searchService;
        private readonly ReportService? _reportService;
        public ConsoleMenu(CustomerService? customerService, SearchService searchService, ReportService reportService)
        {
            _customerService = customerService;
            _searchService = searchService;
            _reportService = reportService;
        }

        public void HandleCustomerMenu()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n Ügyfelek kezelése ");
                Console.WriteLine();
                Console.WriteLine("1. Új ügyfél rögzítése");
                Console.WriteLine("2. Összes ügyfél listázása");
                Console.WriteLine("3. Egy ügyfél részletes adatainak megjelenítése");
                Console.WriteLine("4. Ügyfél adatainak módosítása");
                Console.WriteLine("5. Ügyfél által feladott csomagok");
                Console.WriteLine("6. Ügyfélnek címzett csomagok");
                Console.WriteLine("0. Vissza a főmenübe");

                Console.Write("\nVálasztás: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _customerService?.AddCustomer();
                        break;

                    case "2":
                        _customerService?.ShowCustomer();
                        break;

                    case "3":
                        _customerService?.ShowCustomerDetails();
                        break;

                    case "4":
                        _customerService?.UpdateCustomerData();
                        break;

                    case "5":
                        _customerService?.ShowCustomerSender();
                        break;

                    case "6":
                        _customerService?.ShowCustomerRecipient();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Érvénytelen választás!");
                        Console.WriteLine("Add be újból a számot");
                        break;
                }
            }
        }

        public void SearchPackages()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("\n CSOMAGOK KERESÉSE ");
                Console.WriteLine("1. Keresés nyomkövetési azonosító alapján");
                Console.WriteLine("2. Keresés feladó alapján");
                Console.WriteLine("3. Keresés címzett alapján");
                Console.WriteLine("4. Keresés állapot alapján");
                Console.WriteLine("5. Keresés csomagtípus alapján");
                Console.WriteLine("6. Keresés feladási időszak alapján");
                Console.WriteLine("0. Vissza a főmenübe");

                Console.Write("\nVálasztás: ");
                string? choice = Console.ReadLine();

                switch (choice)
                { 

                case "1":
                   _searchService?.SearchByTrackingId();
                   break;

                case "2":
                   _searchService?.SearchBySender(_customerService!);
                   break;   

                case "3":
                   _searchService?.SearchByRecipient(_customerService!);
                   break;

                case "4":
                   _searchService?.SearchByStatus();
                   break;

                case "5":
                   _searchService?.SearchByPackageType();
                   break;

                case "6":
                   _searchService?.SearchByPostingPeriod();
                   break;

                case "0":
                   running = false;
                   break;

                default:
                    Console.WriteLine("Érvénytelen választás!");
                    Console.WriteLine("Add be újból a számot");
                    break;
                }
            }
        }

        public void Reports()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("\n RIPORTOK ÉS KIMUTATÁSOK ");
                Console.WriteLine("1. Csomagok száma állapotonként");
                Console.WriteLine("2. Csomagtípus-statisztika");
                Console.WriteLine("3. Öt legaktívabb feladó");
                Console.WriteLine("4. Sikertelen kézbesítések okai");
                Console.WriteLine("5. Időszaki feladási kimutatás");
                Console.WriteLine("6. Speciális kezelést igénylő aktív csomagok");
                Console.WriteLine("0. Vissza a főmenübe");

                Console.Write("\nVálasztás: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {

                    case "1":
                        _reportService?.NumberOfPackagesByStatus();
                        break;

                    case "2":
                        _reportService?.PackageTypeStatistics();
                        break;

                    case "3":
                        _reportService?.TopFiveSenders();
                        break;

                    case "4":
                        _reportService?.ReasonsForUnsuccessfulDeliveries();
                        break;

                    case "5":
                        _reportService?.TimePeriodReport();
                        break;

                    case "6":
                        _reportService?.SpecialHandlingPackages();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Érvénytelen választás!");
                        Console.WriteLine("Add be újból a számot");
                        break;
                }
            }
        }
    }
}
