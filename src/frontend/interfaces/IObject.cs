using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fleasociety
{
    // nvg engine object system!
    public interface IObject
    {
        public string title { get; }
        public bool Update(GameTime gameTime, bool handleInput);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice);
    }
}
