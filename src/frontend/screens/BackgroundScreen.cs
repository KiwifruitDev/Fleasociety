using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fleasociety
{
    /// <summary>
    /// This is the background screen, it draws a scrolling tiled pattern.
    /// </summary>
    public class BackgroundScreen : IScreen
    {
        /// <summary>
        /// The title of the screen. This is displayed on the header bar.
        /// </summary>
        public string title { get; } = "Background";
        public int layer { get; set; } = 0;
        private static int totalCount = 1024; // TODO: Calculate this based on screen size.
        public bool Update(GameTime gameTime, bool handleInput)
        {
            // Input.
            if(handleInput)
            {
                // Detect clicks.
                if (Input.MouseState.LeftButton == ButtonState.Released && Input.LastMouseState.LeftButton == ButtonState.Pressed) {
                    // Play a sound.
                    GlobalContent.PlaySound("Hover");
                    return true;
                }
            }
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw background with new hue.
            spriteBatch.Draw(GlobalContent.GetTexture("Pixel"), new Rectangle(GlobalGraphics.Scale((int)-GlobalGraphics.drawOffset.X), GlobalGraphics.Scale((int)-GlobalGraphics.drawOffset.Y), (int)GlobalGraphics.preferredResolution.X, (int)GlobalGraphics.preferredResolution.Y), ThemeManager.GetColor("BackgroundScreen"));
            
            if(!bool.Parse(SaveData.saveValues["DisableMotion"]))
            {
                // Draw the background.
                // This is done by drawing four background layers, each with a different direction for the illusion of infinite scrolling.
                Texture2D tile = GlobalContent.GetTexture("Tile");
                for(int x = 0; x < totalCount; x += tile.Width)
                {
                    for(int y = 0; y < totalCount; y += tile.Height)
                    {
                        spriteBatch.Draw(tile, new Rectangle(GlobalGraphics.Scale(x), GlobalGraphics.Scale(y), GlobalGraphics.Scale(tile.Width), GlobalGraphics.Scale(tile.Height)), ThemeManager.GetColor("TileBackgroundScreen"));
                    }
                }
            }
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            GlobalContent.AddTexture("Tile", ThemeManager.LoadLayeredContent<Texture2D>("graphics/tile"));
        }
    }
}
