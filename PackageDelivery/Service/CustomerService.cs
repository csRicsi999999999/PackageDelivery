using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        bool isValid = false;
        public List<Customer> GetAllCustomers()
        {
            return _db.Customers.ToList();
        }
        public void AddCustomer()
        {
            Console.WriteLine("Add meg az ügyfél nevét: ");
            string? name = null;

            while (!isValid)
            {
                name = Console.ReadLine()!;
                try
                {
                    if (int.TryParse(name, out _))
                    {
                        throw new ArgumentException("Csak betűt adhatsz meg");
                    }
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new ArgumentException("Az ügyfél neve kötelező.");
                    }
                    isValid = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("\nAdd meg az ügyfél nevét újra: ");
                }
            }

            Console.WriteLine("Add meg az e-mail címét: ");
            string? email = null;

            isValid = false;

            while (!isValid)
            {
                email = Console.ReadLine()!;
                try
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        throw new ArgumentException("Az ügyfél e-mail címe kötelező.");
                    }
                    if (!email.Contains('@') || email.First() == '@' || email.Last() == '@')
                    {
                        throw new ArgumentException("Érvénytelen e-mail cím.");
                    }
                    isValid = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("\nAdd meg az e-mail címét újra: ");
                }
            }

            Console.WriteLine("Add meg a telefonszámát: ");
            string? phoneNumber = null;

            isValid = false;

            while (!isValid)
            {
                phoneNumber = Console.ReadLine()!;
                try
                {
                    if (string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        throw new ArgumentException("Az ügyfél telefonszáma kötelező.");
                    }
                    isValid = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("\nAdd meg a telefonszámát újra: ");
                }
            }
             
            _db.Customers.Add(new Customer
            {
                Name = name!.Trim(),
                Email = email!.Trim(),
                PhoneNumber = phoneNumber!.Trim()
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
            var customers = GetAllCustomers();
            Console.WriteLine("Ügyfelek listája: ");
            foreach (var c in customers)
            {
                Console.WriteLine($"{c.Id}: {c.Name}");
            }

            Console.WriteLine("Add meg az ügyfél ID-ját: ");

            int customerId = 0;
            isValid = false;

            while (!isValid)
            {
                try
                {
                    if (!int.TryParse(Console.ReadLine(), out customerId))
                    {
                        throw new ArgumentException("Számot adj meg");
                    }
                    if (customerId < 1 || customerId > _db.Customers.Count())
                    {
                        throw new ArgumentException("Érvénytelen választás");
                    }
                    isValid = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("\nAdd meg az ügyfél ID-ját újra: ");
                }
            }

            foreach(var item in _db.Customers.Where(c => c.Id == customerId))
            {
                Console.WriteLine($"\nÜgyfél neve: {item.Name}");
                Console.WriteLine($"E-mail cím: {item.Email}");
                Console.WriteLine($"Telefonszám: {item.PhoneNumber}");
            }

            Console.WriteLine("\nNyomj egy ENTERT a kilépéshez");
            Console.ReadKey();

        }
        public void UpdateCustomerData()
        {

            string? choice = null;
            string? result = "";
            isValid = false;

            while (!isValid)
            {
                try
                {
                    Console.WriteLine("Melyik adatot szeretnéd módosítani?: ");
                    Console.WriteLine("1-Név - 2-E-mail cím - 3-Telefonszám");
                    choice = Console.ReadLine();

                    if (choice != "1" && choice != "2" && choice != "3")
                    {
                        throw new ArgumentException("Érvénytelen választás");
                    }
                    isValid = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("\nAdd meg a választásod újra: ");
                    choice = Console.ReadLine();
                }
            }

            var customers = GetAllCustomers();
            Console.WriteLine("Ügyfelek listája: ");
            foreach (var item in customers)
            {
                Console.WriteLine($"{item.Id}: {item.Name}");
            }

            int customerId = 0;
            bool isIdValid = false;
            while (!isIdValid)


                while (!isIdValid)
                {
                    Console.Write("Add meg az ügyfél ID-ját: ");
                    if (int.TryParse(Console.ReadLine(), out customerId))
                    {
                        isIdValid = true;
                    }
                    else
                    {
                        Console.WriteLine("Érvénytelen ID! Csak számot adj meg.");
                    }
                }

            var customer = _db.Customers.Find(customerId);
            if (customer == null)
            {
                Console.WriteLine("Az ügyfél nem található.");
                return;
            }

            switch (choice)
            {
                case "1":
                    Console.Write("Add meg az új nevet: ");
                    string newName = Console.ReadLine()!;
                    customer.Name = newName;
                    result = "Az ügyfél neve frissítve.";
                    break;

                case "2":
                    Console.Write("Add meg az új e-mail címet: ");
                    string newEmail = Console.ReadLine()!;
                    customer.Email = newEmail;
                    result = "Az ügyfél e-mail címe frissítve.";
                    break;

                case "3":
                    Console.Write("Add meg az új telefonszámot: ");
                    string newPhoneNumber = Console.ReadLine()!;
                    customer.PhoneNumber = newPhoneNumber;
                    result = "Az ügyfél telefonszáma frissítve.";
                    break;
            }
            _db.SaveChanges();
            Console.WriteLine(result);
        }

        public void ShowCustomerSender()
        {
            isValid = false;

            var customers = GetAllCustomers();
            Console.WriteLine("Ügyfelek listája: ");
            foreach (var item in customers)
            {
                Console.WriteLine($"{item.Id}: {item.Name}");
            }

            Console.Write("\nAdd meg az ügyfél ID-ját: ");
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
            
            var customers = GetAllCustomers();
            Console.WriteLine("Ügyfelek listája: ");
            foreach (var item in customers)

            Console.Write("\nAdd meg az ügyfél ID-ját: ");
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
