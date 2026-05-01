using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class Ticket
    {
        private byte customerId; // displayed as #1, #2, etc.

        public Ticket(byte customerId)
        {
            this.customerId = customerId;
        }
    }
    public class TicketScreen : IScreen {
        public string title { get; set; } = "Tickets";
        public int layer { get; set; } = 4;
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