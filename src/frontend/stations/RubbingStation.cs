using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class RubbingStation : IStation
    {
        public string title { get; set; } = "Rub Station";
        public int order { get; set; } = 3;
        public bool attention { get; set; } = false;
        public Color color { get; set; } = Color.DarkCyan;
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