using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Globalization;
using System;
using System.Linq;

namespace Fleasociety
{
    /// <summary>
    /// Debug screen.
    /// </summary>
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
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58) && Input.MouseState.Y <= GlobalGraphics.Scale(58+6))
                        {
                            // Search for the next logical entry in locales/*.json
                            string currentLocale = L.GetLocale().name;
                            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", L.localeFolder);
                            if(!File.Exists(Path.Combine(path, currentLocale + ".json")))
                                currentLocale = L.defaultLocale;
                            string[] files = Directory.GetFiles(path, "*.json");
                            string newLocale = currentLocale;
                            for(int i = 0; i < files.Length; i++)
                            {
                                if(files[i].EndsWith(currentLocale + ".json"))
                                {
                                    // Get the next logical entry.
                                    if(i + 1 < files.Length)
                                    {
                                        newLocale = Path.GetFileNameWithoutExtension(files[i + 1]);
                                        break;
                                    }
                                    // Wrap around to the first entry.
                                    newLocale = Path.GetFileNameWithoutExtension(files[0]);
                                    break;
                                }
                            }
                            newLocale = newLocale.Replace(".json", "");
                            L.LoadLocale(newLocale);
                            GlobalContent.PlaySound("Select");
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+9) && Input.MouseState.Y <= GlobalGraphics.Scale(58+9+6))
                        {
                            // Toggle user resizing.
                            if(Frontend.instance != null)
                                Frontend.instance.Window.AllowUserResizing = !Frontend.instance.Window.AllowUserResizing;
                            GlobalContent.PlaySound("Select");
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+(9*2)) && Input.MouseState.Y <= GlobalGraphics.Scale(58+(9*2)+6))
                        {
                            // Toggle screen scale.
                            if(SaveData.saveValues["ScreenScale"] == "1")
                                SaveData.saveValues["ScreenScale"] = "2";
                            else if(SaveData.saveValues["ScreenScale"] == "2")
                                SaveData.saveValues["ScreenScale"] = "3";
                            else if(SaveData.saveValues["ScreenScale"] == "3")
                                SaveData.saveValues["ScreenScale"] = "4";
                            else if(SaveData.saveValues["ScreenScale"] == "4")
                                SaveData.saveValues["ScreenScale"] = "1";
                            else
                                SaveData.saveValues["ScreenScale"] = "2";
                            GlobalContent.PlaySound("Select");
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+(9*3)) && Input.MouseState.Y <= GlobalGraphics.Scale(58+(9*3)+6))
                        {
                            // Toggle speed boost.
                            Debug.debugSpeedBoost++;
                            if(Debug.debugSpeedBoost > 10)
                                Debug.debugSpeedBoost = 2;
                            GlobalContent.PlaySound("Select");
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+(9*4)) && Input.MouseState.Y <= GlobalGraphics.Scale(58+(9*4)+6))
                        {
                            // Toggle draw offset.
                            AspectRatio current = GlobalGraphics.GetAspectRatio();
                            // Get the index of the current AspectRatio
                            int index = AspectRatio.All.IndexOf(current);
                            // Get the next AspectRatio
                            AspectRatio next = AspectRatio.All[(index + 1) % AspectRatio.All.Count];
                            // Set the next AspectRatio
                            GlobalGraphics.SetAspectRatio(next);
                            GlobalContent.PlaySound("Select");
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+(9*5)) && Input.MouseState.Y <= GlobalGraphics.Scale(58+(9*5)+6))
                        {
                            // Cycle theme.
                            GlobalContent.PlaySound("Select");
                            string name = ThemeManager.activeTheme.name;
                            // Check to see if it's an internal theme.
                            for(int i = 0; i < DefaultThemes.themes.Count(); i++)
                            {
                                if(name == DefaultThemes.themes[i].name)
                                {
                                    if(i + 1 < DefaultThemes.themes.Count())
                                    {
                                        ThemeManager.ApplyTheme(DefaultThemes.themes[i + 1]);
                                        return true;
                                    }
                                    break;
                                }
                            }
                            ThemeManager.ApplyTheme(DefaultThemes.themes[0]);
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+(9*6)) && Input.MouseState.Y <= GlobalGraphics.Scale(58+(9*6)+6))
                        {
                            // Save.
                            SaveData.Save();
                            GlobalContent.PlaySound("Select");
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+(9*7)) && Input.MouseState.Y <= GlobalGraphics.Scale(58+(9*7)+6))
                        {
                            // Toggle hidden verbose.
                            GlobalContent.PlaySound("Select");
                            SaveData.saveValues["HiddenVerbose"] = (!bool.Parse(SaveData.saveValues["HiddenVerbose"])).ToString(CultureInfo.InvariantCulture);
                            SaveData.Save();
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+(9*8)) && Input.MouseState.Y <= GlobalGraphics.Scale(58+(9*8)+6))
                        {
                            // Open console.txt.
                            GlobalContent.PlaySound("Select");
                            string consoleLogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "console.txt");
                            if(File.Exists(consoleLogFile))
                                System.Diagnostics.Process.Start("notepad", consoleLogFile);
                            return true;
                        }
                        if(Input.MouseState.Y >= GlobalGraphics.Scale(58+(9*9)) && Input.MouseState.Y <= GlobalGraphics.Scale(58+(9*9)+6))
                        {
                            // Unlock FPS.
                            GlobalContent.PlaySound("Select");
                            if(Frontend.instance != null)
                                Frontend.instance.SetFPSUnlock(Frontend.instance.IsFixedTimeStep);
                            return true;
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
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F3: Toggle Debug Mode", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F6: Pause", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F7: Advance Frame", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8*2)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F8: Speed Boost", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8*3)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F9: Reload Locales", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8*4)), Color.White);
                GlobalGraphics.DrawShadowedString(spriteBatch, L.FontSmall(), "F10: Unload All Locales", new Vector2(GlobalGraphics.Scale(6), GlobalGraphics.Scale(41) + GlobalGraphics.Scale(8*5)), Color.White);
            }
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
        }
    }
}
