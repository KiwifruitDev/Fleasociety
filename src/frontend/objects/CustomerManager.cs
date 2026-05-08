using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
    public class CustomerObject
    {
        private static readonly Vector2 defaultPosition = new Vector2(320, 0);
        private static readonly Vector2 walkToPosition = new Vector2(0, 0);
        private static readonly int offsetInBackX = 64;
        private static readonly float walkSpeed = 1f;
        private static readonly Point speechBubblePosition = new Point(67, 12);
        private static readonly Point speechButtonPosition = new Point(73, 27);
        private static readonly Point textPosition = new Point(4, 8);
        private static readonly Dictionary<CustomerType, Color> speechButtonColors = new Dictionary<CustomerType, Color>()
        {
            [CustomerType.Regular] = Color.Green,
        };
        private Customer customer;
        private Rectangle rectangle = new Rectangle(0, 24, 128, 192);
        private static Dictionary<string, Texture2D> customerTextures = new Dictionary<string, Texture2D>();
        private Texture2D? texture = null;
        private static Texture2D? speechBubbleTexture = null;
        private static Texture2D? speechButtonTexture = null;
        private bool orderTaken = false;
        private bool arrivedAtCounter = false;
        private bool animate = false;
        private int timestampArrived = 0;
        public CustomerObject(Customer customer)
        {
            this.customer = customer;
            texture = customerTextures[customer.Name];
            timestampArrived = Environment.TickCount;
            if (texture == null)
            {
                ConsoleOutput.WriteLine($"Texture for customer {customer.Name} not found!", Color.Red);
            }
            animate = true;
            /*
            else
            {
                // Set rectangle size to texture size
                rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            }
            */
        }
        public bool Update(GameTime gameTime, bool handleInput)
        {
            if(animate)
            {
                if(!arrivedAtCounter)
                {
                    Vector2 position = new Vector2(rectangle.X, rectangle.Y);
                    position = Vector2.Lerp(position, walkToPosition, walkSpeed);
                    if(Vector2.Distance(position, walkToPosition) < 0.5f)
                    {
                        position = walkToPosition;
                        animate = false;
                        arrivedAtCounter = true;
                    }
                    rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                }
            }
            if(handleInput)
            {
                if (speechButtonTexture != null)
                {
                    Rectangle buttonPos = GlobalGraphics.Scale(new Rectangle(rectangle.X + speechButtonPosition.X, rectangle.Y + speechButtonPosition.Y, speechButtonTexture.Width, speechButtonTexture.Height));
                    if(buttonPos.Contains(Input.startClick) && buttonPos.Contains(Input.MouseState.Position)
                        && Input.LastMouseState.LeftButton == ButtonState.Pressed
                        && Input.MouseState.LeftButton == ButtonState.Released)
                    {
                        GlobalContent.PlaySound("RenderComplete");
                        orderTaken = true;
                        TicketManager.Add(new Ticket());
                    }
                }
            }
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture == null)
                texture = customerTextures[customer.Name];
            if(speechBubbleTexture == null)
                speechBubbleTexture = GlobalContent.GetTexture($"SpeechBubble");
            if(speechButtonTexture == null)
                speechButtonTexture = GlobalContent.GetTexture($"SpeechButton");
            if (texture != null
                && speechBubbleTexture != null
                && speechButtonTexture != null)
            {
                spriteBatch.Draw(texture, GlobalGraphics.Scale(rectangle), Color.White);
                // tooltip
                if(GlobalGraphics.Scale(rectangle).Contains(Input.MouseState.Position))
                {
                    Global.tooltip = customer.Name;
                }
                if(arrivedAtCounter && !orderTaken)
                {
                    spriteBatch.Draw(speechBubbleTexture, GlobalGraphics.Scale(new Rectangle(rectangle.X + speechBubblePosition.X, rectangle.Y + speechBubblePosition.Y, speechBubbleTexture.Width, speechBubbleTexture.Height)), Color.White);
                    Rectangle buttonPos = new Rectangle(rectangle.X + speechButtonPosition.X, rectangle.Y + speechButtonPosition.Y, speechButtonTexture.Width, speechButtonTexture.Height);
                    spriteBatch.Draw(speechButtonTexture, GlobalGraphics.Scale(buttonPos), speechButtonColors[customer.Type]);
                    GlobalGraphics.DrawString(spriteBatch, L.FontSmall(), "Take order", GlobalGraphics.Scale(new Vector2(buttonPos.X + textPosition.X, buttonPos.Y + textPosition.Y)), Color.White);
                    if(GlobalGraphics.Scale(buttonPos).Contains(Input.MouseState.Position))
                    {
                        Global.tooltip = "Take order";
                    }
                }
            }
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            customerTextures.Clear();
            foreach (Customer customer in CustomerManager.GetAllCustomers())
            {
                // Load customer textures
                Texture2D? customerTexture = GlobalContent.AddTexture($"Customer{customer.Name}", ThemeManager.LoadLayeredContent<Texture2D>($"graphics/customers/{customer.Name.ToLower()}"));
                if (customerTexture == null)
                    ConsoleOutput.WriteLine($"Texture for customer {customer.Name} not found!", Color.Red);
                else
                    customerTextures.Add(customer.Name, customerTexture);
            }
            speechBubbleTexture = GlobalContent.AddTexture($"SpeechBubble", ThemeManager.LoadLayeredContent<Texture2D>($"graphics/stations/order/speechbubble"));
            speechButtonTexture = GlobalContent.AddTexture($"SpeechButton", ThemeManager.LoadLayeredContent<Texture2D>($"graphics/stations/order/speechbutton"));
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
        
        public static List<CustomerObject> customerObjects = new List<CustomerObject>();
        public static CustomerObject AddCustomerObject(Customer customer)
        {
            CustomerObject customerObject = new CustomerObject(customer);
            customerObjects.Add(customerObject);
            return customerObject;
        }
        public static bool Update(GameTime gameTime, bool handleInput)
        {
            bool handled = !handleInput;
            foreach (CustomerObject customer in customerObjects)
            {
                handled = customer.Update(gameTime, !handled);
            }
            return handled;
        }
        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (CustomerObject customer in customerObjects)
            {
                customer.Draw(gameTime, spriteBatch);
            }
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            CustomerObject.LoadContent(contentManager, graphicsDevice);
        }
    }
}