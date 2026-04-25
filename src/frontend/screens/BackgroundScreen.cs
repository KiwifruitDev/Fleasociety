using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KMGEngine
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
        private static int totalCount = 2048; // TODO: Calculate this based on screen size.
        private int scrollX = 0;
        private int scrollY = 0;
        private float counter = 360;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            // Move background.
            counter -= 0.125f;
            if(counter <= 0)
            {
                // Print time taken to scroll.
                //ConsoleOutput.WriteLine("Time taken to scroll: " + gameTime.TotalGameTime.TotalSeconds);
                counter = 360;
            }
            // I started off making this scroll from top right to bottom left.
            // Then I changed it to scroll in a circular motion.
            // Now it scrolls in some sort of zig-zag pattern.
            scrollX = (-totalCount/4) - (int)(Math.Sin(counter * Math.PI / -90) * (GlobalGraphics.scaledWidth/GlobalGraphics.scale) / 2);
            scrollY = (-totalCount/4) - (int)(Math.Cos(counter * Math.PI / -180) * (GlobalGraphics.scaledHeight/GlobalGraphics.scale) / 2);
            // Input.
            if(handleInput)
            {
                // Detect clicks.
                if (Input.LastMouseState.LeftButton == ButtonState.Released && Input.MouseState.LeftButton == ButtonState.Pressed) {
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
                // End existing spritebatch
                spriteBatch.End();

                float mX = GlobalGraphics.drawOffset.X;
                float mY = GlobalGraphics.drawOffset.Y;

                // Create matrix
                Matrix matrix = Matrix.CreateTranslation(mX, mY, 0);
                
                // Create spritebatch with panning (and respect draw offset)
                spriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null, null, null, matrix);

                // Draw the background.
                // This is done by drawing four background layers, each with a different direction for the illusion of infinite scrolling.
                Texture2D tile = GlobalContent.GetTexture("Tile");
                for(int x = 0; x < totalCount; x += tile.Width)
                {
                    for(int y = 0; y < totalCount; y += tile.Height)
                    {
                        spriteBatch.Draw(tile, new Rectangle(GlobalGraphics.Scale(x + scrollX), GlobalGraphics.Scale(y + scrollY), GlobalGraphics.Scale(tile.Width), GlobalGraphics.Scale(tile.Height)), ThemeManager.GetColor("TileBackgroundScreen"));
                    }
                }

                // End offset spritebatch
                spriteBatch.End();
                // Remake spritebatch
                spriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null, null, null, Matrix.CreateTranslation(GlobalGraphics.Scale(GlobalGraphics.drawOffset.X), GlobalGraphics.Scale(GlobalGraphics.drawOffset.Y), 0));
            }
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            GlobalContent.AddTexture("Tile", ThemeManager.LoadLayeredContent<Texture2D>("graphics/tile"));
        }
    }
}
