using PackageDelivery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageDelivery.Service
{
    internal class InputHelper
    {
        public static (DateTime StartDate, DateTime EndDate) ReadDateRange()
        {
            DateTime startDate = DateTime.MinValue;
            bool isValidStartDate = false;

            while (!isValidStartDate)
            {
                Console.WriteLine("Add meg a kezdő dátumot (YYYY-MM-DD): ");
                string? input = Console.ReadLine();

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
                string? input = Console.ReadLine();

                if (DateTime.TryParse(input, out endDate))
                {
                    if (startDate.Date > endDate.Date)
                    {
                        Console.WriteLine("A kezdő dátumnak kisebbnek vagy egyenlőnek kell lennie, mint a záró dátumnak!");
                    }
                    else
                    {
                        isValidEndDate = true;
                    }
                }
                else
                {
                    Console.WriteLine("Hibás dátumformátum! Kérlek add meg újra.");
                }
            }

            DateTime normalizedStart = startDate.Date;
            DateTime normalizedEnd = endDate.Date.AddDays(1).AddTicks(-1);

            return (normalizedStart, normalizedEnd);
        }

        public static Customer SelectCustomer(List<Customer> customers, string prompt, int? excludedCustomerId = null, bool showList = true)
        {
            if (showList)
            {
                Console.WriteLine("\nElérhető ügyfelek:");
                for (int i = 0; i < customers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {customers[i].Name} ({customers[i].Email})");
                }
            }

            int selectedNumber = 0;
            bool isValid = false;

            while (!isValid)
            {
                Console.Write($"\n{prompt}: ");
                try
                {
                    if (!int.TryParse(Console.ReadLine(), out selectedNumber))
                    {
                        throw new ArgumentException("Számot adj meg!");
                    }

                    if (selectedNumber < 1 || selectedNumber > customers.Count)
                    {
                        throw new ArgumentException($"1 és {customers.Count} közötti sorszámot adj meg!");
                    }

                    Customer chosen = customers[selectedNumber - 1];

                    if (excludedCustomerId.HasValue && chosen.Id == excludedCustomerId.Value)
                    {
                        throw new ArgumentException("A feladó és a címzett nem lehet ugyanaz az ügyfél!");
                    }

                    return chosen;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return customers[0];
        }
    }
}
