using PackageDelivery.Data;
using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageDelivery.Service
{
    public class CustomerService
    {
        private readonly AppDbContext _db;
        public CustomerService(AppDbContext db)
        {
            _db = db;
        }
        public List<Customer> GetAllCustomers()
        {
            return _db.Customers.ToList();
        }
        public string AddCustomer(string name, string email, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phoneNumber))
                return "A nev, az e-mail-cim es a telefonszam kotelezo.";

            _db.Customers.Add(new Customer
            {
                Name = name.Trim(),
                Email = email.Trim(),
                PhoneNumber = phoneNumber.Trim()
            });
            _db.SaveChanges();
            return "Az ugyfel rogzitve.";
        }
        public void ShowCustomer()
        {
            foreach (Customer item in _db.Customers)
                Console.WriteLine($"{item.Id}: {item.Name} - {item.Email} - {item.PhoneNumber}");
        }
        public void ShowCustomerDetails()
        {
            Console.WriteLine("Add meg az ügyfél ID-ját: ");
            string customerId = Console.ReadLine() ?? "";

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

            string choice = Console.ReadLine() ?? "";
            string result = "";

            switch (choice)
            {
                case "1":
                    Console.Write("Add meg az ügyfél ID-ját: ");
                    int customerId = int.Parse(Console.ReadLine() ?? "");
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
                    Console.Write("Add meg az ügyfél ID-ját: ");
                    int customerId3 = int.Parse(Console.ReadLine() ?? "0");
                    var customer3 = _db.Customers.Find(customerId3);
                    if (customer3 != null)
                    {
                        Console.Write("Add meg az új telefonszámot: ");
                        string newPhoneNumber = Console.ReadLine() ?? "";
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
    }
}
