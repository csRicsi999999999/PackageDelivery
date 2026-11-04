using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PackageDelivery.Service
{
    public class CustomerService
    {
        private readonly AppDbContext _db;
        PackageService? _packageService;
        public CustomerService(AppDbContext db)
        {
            _db = db;
            _packageService = new PackageService(_db);
        }
        
        public List<Customer> GetAllCustomers()
        {
            return _db.Customers.ToList();
        }
        public void AddCustomer()
        {
            Console.WriteLine("Add meg az ügyfél nevét: ");
            string name = Console.ReadLine()!;

            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Az ügyfél neve kötelező.");
                Console.WriteLine("\nAdd meg az ügyfél nevét újra: ");
                name = Console.ReadLine()!;
            }

            Console.WriteLine("Add meg az e-mail címét: ");
            string email = Console.ReadLine()!;

            while (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Az ügyfél e-mail címe kötelező.");
                Console.WriteLine("\nAdd meg az e-mail címét újra: ");
                email = Console.ReadLine()!;
            }

            if (email.First() == '@' || email.Last() == '@' || !email.Contains('@'))
            {
                Console.WriteLine("Érvénytelen e-mail cím.");
                Console.WriteLine("\nAdd meg az e-mail címét újra: ");
                email = Console.ReadLine()!;
            }

            Console.WriteLine("Add meg a telefonszámát: ");
            string phoneNumber = Console.ReadLine()!;

            while (string.IsNullOrWhiteSpace(phoneNumber))
            {
                Console.WriteLine("Az ügyfél telefonszáma kötelező.");
                Console.WriteLine("\nAdd meg a telefonszámát újra: ");
                phoneNumber = Console.ReadLine()!;
            }
             
            _db.Customers.Add(new Customer
            {
                Name = name.Trim(),
                Email = email.Trim(),
                PhoneNumber = phoneNumber.Trim()
            });
            _db.SaveChanges();

            Console.WriteLine("Az ugyfel rogzitve.");
            

        }
        public void ShowCustomer()
        {
            foreach (Customer item in _db.Customers)
                Console.WriteLine($"{item.Id}: {item.Name} - {item.Email} - {item.PhoneNumber}");
        }
        public void ShowCustomerDetails()
        {
            Console.WriteLine("Ügyfelek listája: ");
            for (int i = 0; i < _db.Customers.Count(); i++)
            {
                Console.WriteLine(GetAllCustomers()[i].Id + ": " + GetAllCustomers()[i].Name);
            }

            Console.WriteLine("Add meg az ügyfél ID-ját: ");
            string customerId = Console.ReadLine()!;

            var customer = _db.Customers.Find(int.Parse(customerId));

            if (customer != null)
            {
                Console.WriteLine($"{customer.Id}: {customer.Name} - {customer.Email} - {customer.PhoneNumber}");
            }
            else
            {
                Console.WriteLine("Az ügyfél nem található.");
            }
        }
        public void UpdateCustomerData()
        {
            Console.WriteLine("Melyik adatot szeretnéd módosítani?: ");
            Console.WriteLine("1-Név - 2-E-mail cím - 3-Telefonszám");

            string? choice = Console.ReadLine();
            string? result = "";

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Ügyfelek listája: ");
                    for (int i = 0; i < _db.Customers.Count(); i++)
                    {
                        Console.WriteLine(GetAllCustomers()[i].Id + ": " + GetAllCustomers()[i].Name);
                    }

                    Console.Write("Add meg az ügyfél ID-ját: ");
                    int customerId = int.Parse(Console.ReadLine()!);
                    var customer = _db.Customers.Find(customerId);
                    if (customer != null)
                    {
                        Console.Write("Add meg az új nevet: ");
                        string newName = Console.ReadLine() ?? "";
                        customer.Name = newName;
                        _db.SaveChanges();
                        result = "Az ügyfél neve frissítve.";
                    }
                    else
                    {
                        result = "Az ügyfél nem található.";
                    }
                    break;

                case "2":
                    Console.WriteLine("Ügyfelek listája: ");
                    for (int i = 0; i < _db.Customers.Count(); i++)
                    {
                        Console.WriteLine(GetAllCustomers()[i].Id + ": " + GetAllCustomers()[i].Name);
                    }

                    Console.Write("Add meg az ügyfél ID-ját: ");
                    int customerId2 = int.Parse(Console.ReadLine() ?? "0");
                    var customer2 = _db.Customers.Find(customerId2);
                    if (customer2 != null)
                    {
                        Console.Write("Add meg az új e-mail címet: ");
                        string newEmail = Console.ReadLine() ?? "";
                        customer2.Email = newEmail;
                        _db.SaveChanges();
                        result = "Az ügyfél e-mail címe frissítve.";
                    }
                    else
                    {
                        result = "Az ügyfél nem található.";
                    }
                    break;

                case "3":
                    Console.WriteLine("Ügyfelek listája: ");
                    for (int i = 0; i < _db.Customers.Count(); i++)
                    {
                        Console.WriteLine(GetAllCustomers()[i].Id + ": " + GetAllCustomers()[i].Name);
                    }

                    Console.Write("Add meg az ügyfél ID-ját: ");
                    int customerId3 = int.Parse(Console.ReadLine()!);
                    var customer3 = _db.Customers.Find(customerId3);
                    if (customer3 != null)
                    {
                        Console.Write("Add meg az új telefonszámot: ");
                        string newPhoneNumber = Console.ReadLine()!;
                        customer3.PhoneNumber = newPhoneNumber;
                        _db.SaveChanges();
                        result = "Az ügyfél telefonszáma frissítve.";
                    }
                    else
                    {
                        result = "Az ügyfél nem található.";
                    }
                    break;
            }

            Console.WriteLine(result);
        }

        public void ShowCustomerSender()
        {
            Console.WriteLine("Ügyfelek listája: ");
            for (int i = 0; i < _db.Customers.Count(); i++)
            {
                Console.WriteLine(GetAllCustomers()[i].Id + ": " + GetAllCustomers()[i].Name);
            }

            var customers = GetAllCustomers();

            Console.Write("\nAdd meg az ügyfél ID-ját: ");
            if (!int.TryParse(Console.ReadLine(), out int senderIndex) || senderIndex < 1 || senderIndex > customers.Count)
            {
                Console.WriteLine("Érvénytelen választás!");
                Console.ReadKey();
            }
            var sender = customers?[senderIndex - 1];

            foreach (var item in _db.Packages
                .Where(p => p.SenderId == sender!.Id)
                .OrderByDescending(p => p.CreatedAt))
            {
                _packageService?.DisplayText(item);
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }

        public void ShowCustomerRecipient()
        {
            Console.WriteLine("Ügyfelek listája: ");
            for (int i = 0; i < _db.Customers.Count(); i++)
            {
                Console.WriteLine(GetAllCustomers()[i].Id + ": " + GetAllCustomers()[i].Name);
            }

            var customers = GetAllCustomers();

            Console.Write("\nAdd meg az ügyfél ID-ját: ");
            if (!int.TryParse(Console.ReadLine(), out int recipientIndex) || recipientIndex < 1 || recipientIndex > customers.Count)
            {
                Console.WriteLine("Érvénytelen választás!");
                Console.ReadKey();
            }
            var recipient = customers?[recipientIndex - 1];

            foreach (var item in _db.Packages
                .Where(p => p.RecipientId == recipient!.Id)
                .OrderByDescending(p => p.CreatedAt))
            {
                _packageService?.DisplayText(item);
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();
        }
    }
}
