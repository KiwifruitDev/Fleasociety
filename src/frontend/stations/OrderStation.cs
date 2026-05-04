using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class CustomerObject
    {
        private static readonly Vector2 defaultPosition = new Vector2(0, 0);
        private Customer customer;
        private Rectangle rectangle = new Rectangle(0, 24, 128, 192);
        private Texture2D? texture = null;
        private bool orderTaken = false;
        private int timestampArrived = 0;
        public CustomerObject(Customer customer)
        {
            this.customer = customer;
            texture = GlobalContent.GetTexture($"Customer{customer.Name}");
            timestampArrived = Environment.TickCount;
            if (texture == null)
            {
                ConsoleOutput.WriteLine($"Texture for customer {customer.Name} not found!", Color.Red);
            }
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
            if(handleInput)
            {
            }
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture == null)
            {
                texture = GlobalContent.GetTexture($"Customer{customer.Name}");
            }
            if (texture != null)
            {
                spriteBatch.Draw(texture, GlobalGraphics.Scale(rectangle), Color.White);
                // tooltip
                if(GlobalGraphics.Scale(rectangle).Contains(Input.MouseState.Position))
                {
                    Global.tooltip = customer.Name;
                }
            }
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            foreach (Customer customer in CustomerManager.GetAllCustomers())
            {
                // Load customer textures
                Texture2D? customerTexture = GlobalContent.AddTexture($"Customer{customer.Name}", ThemeManager.LoadLayeredContent<Texture2D>($"graphics/customers/{customer.Name.ToLower()}"));
                if (customerTexture == null)
                {
                    ConsoleOutput.WriteLine($"Texture for customer {customer.Name} not found!", Color.Red);
                }
            }
        }
    }
    public class OrderStation : IStation
    {
        public string title { get; set; } = "Stations";
        public bool attention { get; set; } = false;
        public List<CustomerObject> customers = new List<CustomerObject>();
        public bool Update(GameTime gameTime, bool handleInput)
        {
            bool handle = handleInput;
            foreach (CustomerObject customer in customers)
            {
                handle = customer.Update(gameTime, handle);
            }
            // press enter to add a customer for testing
            if (handle)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Enter) && Input.LastKeyboardState.IsKeyUp(Keys.Enter))
                {
                    // Add a new customer for testing
                    Customer newCustomer = CustomerManager.GetCustomer(1);
                    customers.Add(new CustomerObject(newCustomer));
                    GlobalContent.PlaySound("AddSource");
                    return true;
                }
            }
            return handle;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (CustomerObject customer in customers)
            {
                customer.Draw(gameTime, spriteBatch);
            }
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            CustomerObject.LoadContent(contentManager, graphicsDevice);
        }
    }
}