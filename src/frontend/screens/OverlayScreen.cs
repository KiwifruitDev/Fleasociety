using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fleasociety
{
    /// <summary>
    /// This is the overlay screen, it draws graphics in place for the hard-coded title text alongside a border.
    /// </summary>
    public class OverlayScreen : IScreen
    {
        /// <summary>
        /// The title of the screen. This is displayed on the header bar.
        /// </summary>
        public string title { get; } = "Border";
        public int layer { get; set; } = 99;
        private float exitOpacity = 0f;
        private bool tooltipVisible = false;
        public Color bgColor = new Color(128, 128, 128);
        public void Show()
        {
        }
        public void Hide()
        {
        }
        public bool Toggle(bool useBool = false, bool toggleTo = false)
        {
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D pixel = GlobalContent.GetTexture("Pixel");
            spriteBatch.Draw(pixel, new Rectangle(0, 0, GlobalGraphics.scaledWidth, GlobalGraphics.scaledHeight), new Color(0, 0, 0, exitOpacity));
            // Draw the border.
            spriteBatch.Draw(pixel, new Rectangle(0, 0, GlobalGraphics.scaledWidth, (int)(4 * GlobalGraphics.scale)), bgColor);
            spriteBatch.Draw(pixel, new Rectangle(0, GlobalGraphics.scaledHeight - (int)(4 * GlobalGraphics.scale), GlobalGraphics.scaledWidth, (int)(4 * GlobalGraphics.scale)), bgColor);
            spriteBatch.Draw(pixel, new Rectangle(0, 0, (int)(4 * GlobalGraphics.scale), GlobalGraphics.scaledHeight), bgColor);
            spriteBatch.Draw(pixel, new Rectangle(GlobalGraphics.scaledWidth - (int)(4 * GlobalGraphics.scale), 0, (int)(4 * GlobalGraphics.scale), GlobalGraphics.scaledHeight), bgColor);
            if (Global.tooltip != "" && tooltipVisible && Global.ready)
                {
                    SpriteFont spriteFont = L.FontSmall();
                    string tooltip = Global.tooltip;
                    Vector2 tooltipSize = spriteFont.MeasureString(tooltip);
                    // Position is relative to mouse position but tries to avoid going off screen
                    Vector2 position = new(Input.MouseState.Position.X + 16, Input.MouseState.Position.Y + 16);
                    // Make sure it doesn't go off the right side of the screen
                    if (position.X + tooltipSize.X + GlobalGraphics.Scale(6) > GlobalGraphics.scaledWidth)
                        position.X = GlobalGraphics.scaledWidth - tooltipSize.X - GlobalGraphics.Scale(6);
                    // Make sure it doesn't go off the left side of the screen
                    if (position.X < GlobalGraphics.Scale(2))
                        position.X = GlobalGraphics.Scale(2);
                    // Make sure it doesn't go off the bottom of the screen
                    if (position.Y + tooltipSize.Y + GlobalGraphics.Scale(2) > GlobalGraphics.scaledHeight)
                        position.Y = GlobalGraphics.scaledHeight - tooltipSize.Y - GlobalGraphics.Scale(2);
                    // Make sure it doesn't go off the top of the screen
                    if (position.Y < GlobalGraphics.Scale(2))
                        position.Y = GlobalGraphics.Scale(2);
                    spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, (int)tooltipSize.X + GlobalGraphics.Scale(2), (int)tooltipSize.Y - GlobalGraphics.Scale(2)), ThemeManager.GetColor("BackgroundTooltip"));
                    // White text
                    GlobalGraphics.DrawShadowedString(spriteBatch, spriteFont, tooltip, new Vector2(position.X + GlobalGraphics.Scale(2), position.Y - GlobalGraphics.Scale(2)), Color.White);
                }
        }
        public bool Update(GameTime gameTime, bool handleInput)
        {
            tooltipVisible = handleInput;
            if(Global.exiting)
            {
                exitOpacity += Global.exitOpacityIncrease;
                if(exitOpacity >= 1 || !Global.fakeExit)
                {
                    Global.exiting = false;
                    if(Frontend.instance != null)
                        Frontend.instance.Exit();
                }
                return true;
            }
            else if(exitOpacity > 0)
            {
                exitOpacity -= Global.exitOpacityIncrease;
                return true;
            }
            // Reset the color
            bgColor = ThemeManager.GetColor("BackgroundOverlayScreen");
            if(!handleInput)
                return false;
            return false;
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
        }
    }
}
