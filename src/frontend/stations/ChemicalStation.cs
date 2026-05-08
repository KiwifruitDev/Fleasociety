using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class ChemicalStation : IStation
    {
        public string title { get; set; } = "Chem Station";
        public int order { get; set; } = 1;
        public bool attention { get; set; } = false;
        public Color color { get; set; } = Color.Maroon;
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