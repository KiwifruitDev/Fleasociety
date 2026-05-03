using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class CustomerObject
    {
        public Customer customer;
        public Texture2D? texture = null;
        public CustomerObject(Customer customer)
        {
            this.customer = customer;
        }
        public bool Update(GameTime gameTime, bool handleInput)
        {
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
        }
    }
    public class OrderStation : IStation
    {
        public string title { get; set; } = "Stations";
        public bool attention { get; set; } = false;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
        }
    }
}