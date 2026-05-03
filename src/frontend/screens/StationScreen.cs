using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class StationScreen : IScreen
    {
        public string title { get; set; } = "Stations";
        public int layer { get; set; } = 3;
        private int stationHeight = 35;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            return StationManager.Update(gameTime, handleInput);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            StationManager.Draw(gameTime, spriteBatch);
            Point screenResolution = GlobalGraphics.GetAspectRatio().preferredResolution;
            Rectangle stationChooser = GlobalGraphics.Scale(new Rectangle(0, screenResolution.Y - stationHeight, screenResolution.X, stationHeight));
            Texture2D pixel = GlobalContent.GetTexture("Pixel");
            spriteBatch.Draw(pixel, stationChooser, ThemeManager.GetColor("VideoPlayerProgressBar"));
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            StationManager.LoadContent(contentManager, graphicsDevice);
        }
    }
}