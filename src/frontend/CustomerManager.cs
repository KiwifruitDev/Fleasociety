using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Fleasociety
{
    public class Customer
    {
        public byte customerId;
        public string name;
        public Customer(byte customerId, string name)
        {
            this.customerId = customerId;
            this.name = name;
        }
    }
    public static class CustomerManager
    {
        private readonly static string customersFile = "game/customers.json";
        private static Customer dummyCustomer = new Customer(0, "Dummy");
        private static List<Customer> customers = new List<Customer>()
        {
            dummyCustomer
        };
        public static void AddCustomer(Customer customer)
        {
            customers.Add(customer);
        }
        public static Customer GetCustomer(byte customerId)
        {
            return customers.Find(c => c.customerId == customerId) ?? dummyCustomer;
        }
        public static Customer GetCustomer(string name)
        {
            return customers.Find(c => c.name == name) ?? dummyCustomer;
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
                }
            }
        }
    }
}