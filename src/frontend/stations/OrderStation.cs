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
        public string title { get; set; } = "Order Station";
        public int order { get; set; } = 0;
        public bool attention { get; set; } = false;
        public Color color { get; set; } = Color.Green;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            // press enter to add a customer for testing
            bool handled = CustomerManager.Update(gameTime, handleInput);
            if (handleInput && !handled)
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
            return handled;
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