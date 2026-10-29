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

        private readonly PackageService? _packageService;
        public ConsoleMenu(CustomerService? customerService, PackageService? packageService)
        {
            _customerService = customerService;
            _packageService = packageService;
        }

        public void HandleCustomerMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("\n ÜGYFELEK KEZELÉSE ");
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
                        Console.Write("Add meg az ügyfél nevét: ");
                        string name = Console.ReadLine() ?? "";

                        Console.Write("Add meg az e-mail címét: ");
                        string email = Console.ReadLine() ?? "";

                        Console.Write("Add meg a telefonszámát: ");
                        string phoneNumber = Console.ReadLine() ?? "";

                        string result = _customerService?.AddCustomer(name, email, phoneNumber) ?? "";

                        Console.WriteLine();
                        Console.WriteLine(result);
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
                        Console.WriteLine("Ez a funkció még nincs megvalósítva.");
                        break;

                    case "6":
                        Console.WriteLine("Ez a funkció még nincs megvalósítva.");
                        break;

                    case "0":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Érvénytelen választás!");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
                    Console.ReadKey();
                }
            }
        }
    }
}
