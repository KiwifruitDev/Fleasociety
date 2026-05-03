using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Globalization;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Fleasociety
{
    public class DebugScreen : IScreen
    {
        public string title { get; set; } = "Debug";
        public int layer { get; set; } = 6;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            if(Debug.GetDebugMode())
            {
                if(handleInput)
                {
                    if(Input.MouseState.LeftButton == ButtonState.Released && Input.LastMouseState.LeftButton == ButtonState.Pressed
                        && Input.MouseState.X >= GlobalGraphics.Scale(137) && Input.MouseState.X <= GlobalGraphics.Scale(303)
                        && Input.startClick.X >= GlobalGraphics.Scale(137) && Input.startClick.X <= GlobalGraphics.Scale(303))
                    {
                        for(int i = 0; i < Debug.GetDebugButtons().Count; i++)
                        {
                            if(Input.MouseState.Y >= GlobalGraphics.Scale(58 + (i * 9)) && Input.MouseState.Y <= GlobalGraphics.Scale(58 + (i * 9) + 9)
                                && Input.startClick.Y >= GlobalGraphics.Scale(58 + (i * 9)) && Input.startClick.Y <= GlobalGraphics.Scale(58 + (i * 9) + 9))
                            {
                                Debug.GetDebugButtons()[i].Click();
                                break;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Interactable
            if(Debug.GetDebugMode())
            {
                /*
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "Locale: " + L.GetLocale().name + " " + L.GetLocale().localizedName,  GlobalGraphics.Scale(new Vector2(137, 58-4)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "User Resizing: " + (Frontend.instance != null && Frontend.instance.Window.AllowUserResizing ? "Enabled" : "Disabled"), GlobalGraphics.Scale(new Vector2(137, 58-4+9)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "Screen Scale: " + SaveData.saveValues["ScreenScale"], GlobalGraphics.Scale(new Vector2(137, 58-4+(9*2))), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "Speed Boost: x" + Debug.debugSpeedBoost, GlobalGraphics.Scale(new Vector2(137, 58-4+(9*3))), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "Draw Offset: " + GlobalGraphics.drawOffset.X.ToString(CultureInfo.InvariantCulture) + ", " + GlobalGraphics.drawOffset.Y.ToString(CultureInfo.InvariantCulture), GlobalGraphics.Scale(new Vector2(137, 58-4+(9*4))), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "Theme: " + ThemeManager.activeTheme.name, GlobalGraphics.Scale(new Vector2(137, 58-4+(9*5))), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "Save", GlobalGraphics.Scale(new Vector2(137, 58-4+(9*6))), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), (bool.Parse(SaveData.saveValues["HiddenVerbose"]) ? "Disable" : "Enable") + " Verbose", GlobalGraphics.Scale(new Vector2(137, 58-4+(9*7))), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "Open console.txt", GlobalGraphics.Scale(new Vector2(137, 58-4+(9*8))), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), (Frontend.instance != null && Frontend.instance.IsFixedTimeStep ? "Unlock" : "Lock") + " FPS and VSync", GlobalGraphics.Scale(new Vector2(137, 58-4+(9*9))), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "Sound test: " + soundTest.ToString(CultureInfo.InvariantCulture), GlobalGraphics.Scale(new Vector2(137, 58-4+(9*10))), Color.White);
                */
                for (int i = 0; i < Debug.GetDebugButtons().Count; i++)
                {
                    GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), Debug.GetDebugButtons()[i].GetText(), GlobalGraphics.Scale(new Vector2(137, 58-4+(9*i))), Color.White);
                    if (Input.MouseState.X >= GlobalGraphics.Scale(137) && Input.MouseState.X <= GlobalGraphics.Scale(303)
                        && Input.MouseState.Y >= GlobalGraphics.Scale(58 + (i * 9)) && Input.MouseState.Y <= GlobalGraphics.Scale(58 + (i * 9) + 9))
                    {
                        Global.tooltip = Debug.GetDebugButtons()[i].GetTooltip();
                    }
                }

                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F3: Toggle Debug Mode", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F6: Pause", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F7: Advance Frame", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8*2)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F8: Speed Boost", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8*3)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F9: Reload Locales", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8*4)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F10: Unload All Locales", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8*5)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontLarge(), "Last SFX: " + GlobalContent.GetLastPlayedSound(), new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(42) + GlobalGraphics.Scale(8*6)), Color.White);
            }
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
        }
    }
}
