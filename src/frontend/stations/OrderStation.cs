using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class OrderStation : IStation
    {
        public string title { get; set; } = "Stations";
        public bool attention { get; set; } = false;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            bool handle = CustomerManager.Update(gameTime, handleInput);
            // press enter to add a customer for testing
            if (handle)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Enter) && Input.LastKeyboardState.IsKeyUp(Keys.Enter))
                {
                    // Add a new customer for testing
                    Customer newCustomer = CustomerManager.GetCustomer(1);
                    CustomerManager.AddCustomerObject(newCustomer);
                    GlobalContent.PlaySound("AddSource");
                    return true;
                }
            }
            return handle;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CustomerManager.Draw(gameTime, spriteBatch);
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            CustomerManager.LoadContent(contentManager, graphicsDevice);
        }
    }
}