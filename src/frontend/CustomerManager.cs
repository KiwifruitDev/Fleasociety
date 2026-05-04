using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Fleasociety
{
    public enum CustomerType
    {
        Regular
    }
    public class Customer
    {
        public byte Id;
        public string Name;
        public CustomerType Type;
        public Customer(byte customerId, string customerName, CustomerType customerType)
        {
            Id = customerId;
            Name = customerName;
            Type = customerType;
        }
    }
    public static class CustomerManager
    {
        private readonly static string customersFile = "game/customers.json";

        // Fallback customer if a customer is not found.
        private static Customer dummyCustomer = new Customer(0, "Dummy", CustomerType.Regular);

        private static List<Customer> customers = new List<Customer>();

        public static Customer GetCustomer(byte customerId)
        {
            Customer? customer = customers.Find(c => c.Id == customerId);
            if (customer == null)
            {
                ConsoleOutput.WriteLine($"Customer with ID {customerId} not found!");
                return dummyCustomer;
            }
            return customer;
        }
        public static Customer GetCustomer(string name)
        {
            Customer? customer = customers.Find(c => c.Name == name);
            if (customer == null)
            {
                ConsoleOutput.WriteLine($"Customer with name {name} not found!");
                return dummyCustomer;
            }
            return customer;
        }
        public static List<Customer> GetAllCustomers()
        {
            return customers;
        }
        public static void LoadCustomers()
        {
            string fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", customersFile);
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                var data = JsonConvert.DeserializeObject<Dictionary<string, List<Customer>>>(json);
                if (data != null && data.ContainsKey("Customers"))
                {
                    customers = data["Customers"];
                    ConsoleOutput.WriteLine($"Loaded {customers.Count} customers.", Color.Green);
                }
            }
            if (customers.Count == 0)
            {
                ConsoleOutput.WriteLine("No customers loaded!", Color.Red);
            }
        }
    }
}