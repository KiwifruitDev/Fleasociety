using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fleasociety
{
    /// <summary>
    /// This is the help screen.
    /// </summary>
    public class ContentScreen : IScreen
    {
        /// <summary>
        /// The title of the screen. This is displayed on the header bar.
        /// </summary>
        public string title { get; } = "Content";
        public int layer { get; set; } = 3;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Main Window
            Texture2D mainwindow = GlobalContent.GetTexture("MainWindow");
            spriteBatch.Draw(mainwindow, new Rectangle(GlobalGraphics.Scale(128-33), GlobalGraphics.Scale(36), GlobalGraphics.Scale(mainwindow.Width), GlobalGraphics.Scale(mainwindow.Height)), Color.White);
            // Draw the center title bar text.
            Vector2 titleSize = L.FontSmall().MeasureString(title);
            GlobalContent.DrawString(spriteBatch, L.FontSmall(), title, new Vector2(GlobalGraphics.Scale(220) - titleSize.X / 2, GlobalGraphics.Scale(37)), Color.White);
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            // Main Window
            GlobalContent.AddTexture("MainWindow", ThemeManager.LoadLayeredContent<Texture2D>("graphics/mainwindow"));
        }
    }
}
