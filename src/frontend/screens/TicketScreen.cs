using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class TicketScreen : IScreen
    {
        public string title { get; set; } = "Tickets";
        public int layer { get; set; } = 4;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            // space bar -> add a ticket for testing.
            if(handleInput && Input.KeyboardState.IsKeyDown(Keys.Space) && Input.LastKeyboardState.IsKeyUp(Keys.Space))
            {
                TicketManager.Add(new Ticket());
                GlobalContent.PlaySound("Disambiguation");
            }
            return TicketManager.Update(gameTime, handleInput);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            TicketManager.Draw(gameTime, spriteBatch);
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            TicketManager.LoadContent(contentManager, graphicsDevice);
        }
    }
}