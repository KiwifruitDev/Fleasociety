using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace Fleasociety
{
    /// <summary>
    /// Show console output through a screen, acts as a modal but doesn't follow modal rules.
    /// </summary>
    public class ConsoleScreen : IScreen
    {
        public string title { get; } = "Console";
        public int layer { get; set; } = 98;
        private bool active = false;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            // Show/hide console when you press f5
            bool returnValue = active;
            if ((Input.KeyboardState.IsKeyDown(Keys.F5) && !Input.LastKeyboardState.IsKeyDown(Keys.F5))
                || Input.KeyboardState.IsKeyDown(Keys.OemTilde) && !Input.LastKeyboardState.IsKeyDown(Keys.OemTilde))
            {
                active = !active;
                ConsoleOutput.ResetScroll();
                if(active)
                {
                    Global.editing = "";
                    GlobalContent.PlaySound("Select");
                }
                else
                {
                    GlobalContent.PlaySound("Back");
                }
            }
            if(!active)
                return returnValue;
            int scrollWeight = Input.KeyboardState.IsKeyDown(Keys.LeftShift) || Input.KeyboardState.IsKeyDown(Keys.RightShift) ? 5 : 1;
            // Scrolling will set ConsoleOutput.paused to true.
            if (Input.MouseState.ScrollWheelValue != Input.LastMouseState.ScrollWheelValue)
            {
                int wheel = Input.MouseState.ScrollWheelValue - Input.LastMouseState.ScrollWheelValue;
                ConsoleOutput.Scroll(wheel * scrollWeight); // automatically divides by 120
            }
            // Clicking or pressing enter will also reset the scroll.
            if (Input.MouseState.LeftButton == ButtonState.Pressed && Input.LastMouseState.LeftButton == ButtonState.Released)
            {
                ConsoleOutput.ResetScroll();
            }
            // And you can scroll with the arrow keys.
            if (!Input.LastKeyboardState.IsKeyDown(Keys.Up) && Input.KeyboardState.IsKeyDown(Keys.Up))
            {
                ConsoleOutput.Scroll(scrollWeight * 120);
            }
            if (!Input.LastKeyboardState.IsKeyDown(Keys.Down) && Input.KeyboardState.IsKeyDown(Keys.Down))
            {
                ConsoleOutput.Scroll(-scrollWeight * 120);
            }
            // Page up scrolls up by 10 lines.
            if (!Input.LastKeyboardState.IsKeyDown(Keys.PageUp) && Input.KeyboardState.IsKeyDown(Keys.PageUp))
            {
                ConsoleOutput.Scroll(ConsoleOutput.maxLines * 120);
            }
            // Page down scrolls down by 10 lines.
            if (!Input.LastKeyboardState.IsKeyDown(Keys.PageDown) && Input.KeyboardState.IsKeyDown(Keys.PageDown))
            {
                ConsoleOutput.Scroll(-ConsoleOutput.maxLines * 120);
            }
            return returnValue;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!active)
                return;
            // Draw the background.
            Texture2D pixel = GlobalContent.GetTexture("Pixel");
            spriteBatch.Draw(pixel, new Rectangle(0, 0, GlobalGraphics.scaledWidth, GlobalGraphics.scaledHeight), ThemeManager.GetColor("BackgroundConsoleScreen"));
            // Draw the center title bar text.
            string newTitle = L.T(0, "Console:Title", L.T(0, "Console:TitleExtra"));
            Vector2 titleSize = L.FontSmall().MeasureString(newTitle);
            GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), newTitle, new Vector2(GlobalGraphics.scaledWidth / 2 - titleSize.X / 2, (6 * GlobalGraphics.scale) - GlobalGraphics.Scale(1)), Color.White);
            // Draw lines.
            float lineHeight = 8 * GlobalGraphics.scale;
            float lineSpacing = 2 * GlobalGraphics.scale;
            float lineY = GlobalGraphics.Scale(16) + lineSpacing;
            try
            {
                foreach (ColoredString line in ConsoleOutput.GetOutput())
                {
                    Vector2 lineSize = L.FontSmall().MeasureString(line.Text);
                    GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), line.Text, new Vector2(GlobalGraphics.Scale(8), lineY), line.Color);
                    lineY += lineHeight;
                }
            }
            catch {}
            // Draw assembly version.
            string version = L.T(0, "Console:Footer", Global.productVersion, ConsoleOutput.scrollAmount > -1 ? (ConsoleOutput.scrollAmount + 1).ToString(CultureInfo.InvariantCulture) : (ConsoleOutput.proxyOutput.Count - ConsoleOutput.maxLines + 1).ToString(CultureInfo.InvariantCulture), ConsoleOutput.proxyOutput.Count.ToString(CultureInfo.InvariantCulture));
            GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), version, new Vector2(GlobalGraphics.Scale(8), lineY), Color.White);
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
        }
    }
}
