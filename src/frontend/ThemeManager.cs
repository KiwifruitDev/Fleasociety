using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KMGEngine
{
    public class Theme
    {
        public string name;
        public string description;
        public string prefix;
        public Dictionary<string, Color> colorTable;
        public Theme(string name, string description, string prefix)
        {
            this.name = name;
            this.description = description;
            this.prefix = prefix;
            this.colorTable = new Dictionary<string, Color>();
        }
        public Theme(string name, string description, string prefix, Dictionary<string, Color> colorTable)
        {
            this.name = name;
            this.description = description;
            this.prefix = prefix;
            this.colorTable = colorTable;
        }
        public Color GetColor(string colorName)
        {
            if (colorTable.ContainsKey(colorName))
            {
                return colorTable[colorName];
            }
            else
            {
                return Color.Transparent;
            }
        }
    }
    public static class DefaultThemes
    {
        public static Theme Nonsensical = new Theme("Nonsensical", "The default theme.", "", new Dictionary<string, Color>() {
            {"ClearColor", new Color(0, 0, 0, 255)},
            {"BackgroundTooltip", new Color(0, 0, 0, 255)},
            {"BackgroundConsoleScreen", new Color(0, 0, 0, 255)},
            {"BackgroundOverlayScreen", new Color(128, 128, 128, 255)},
            {"BackgroundScreen", new Color(64, 64, 64, 255)},
            {"TileBackgroundScreen", new Color(102, 102, 102, 255)},
            {"VideoPlayerProgressBar", new Color(255, 0, 0, 255)},
        });
        public static Theme Anniversary = new Theme("Anniversary", "Celebrate NVG's anniversary!", "themes/anniversary/", new Dictionary<string, Color>() {});
        public static Theme Spooky = new Theme("Spooky", "Trick or treat!", "themes/halloween/", new Dictionary<string, Color>() {
            {"VideoPlayerProgressBar", new Color(60, 0, 128, 255)},
        });
        public static Theme Holiday = new Theme("Holiday", "Merry Christmas!", "themes/holiday/", new Dictionary<string, Color>() {
        });
        public static List<Theme> themes = new List<Theme>()
        {
            Nonsensical,
            Anniversary,
            Spooky,
            Holiday
        };
        public static Theme defaultTheme = Nonsensical;
    }
    public static class ThemeManager
    {
        public static List<Theme> themes = new List<Theme>()
        {
            DefaultThemes.Nonsensical,
            DefaultThemes.Spooky
        };
        public static Theme activeTheme = DefaultThemes.Nonsensical;
        public static void LoadThemes()
        {
            themes.Clear();
            foreach (Theme theme in DefaultThemes.themes)
            {
                themes.Add(theme);
            }
        }
        // Replacement for contentManager.Load<T>(path)
        public static T LoadLayeredContent<T>(string path)
        {
            if (Frontend.instance == null)
            {
                // Can't proceed, MonoGame isn't initialized
                throw new InvalidOperationException("MonoGame isn't initialized. Cannot load content.");
            }
            ContentManager contentManager = Frontend.instance.Content;
            // Make sure xnb file exists
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "Content", activeTheme.prefix + path + ".xnb")))
            {
                //ConsoleOutput.WriteLine($"Fallback: {path} not found in {activeTheme.name}.", Color.Yellow);
                // If not, load from default theme
                return contentManager.Load<T>(path);
            }
            return contentManager.Load<T>(activeTheme.prefix + path);
        }
        public static void ApplyTheme(Theme theme)
        {
            ConsoleOutput.WriteLine($"Theme changed to {theme.name}.", Color.Yellow);
            activeTheme = theme;
            if (Frontend.instance != null)
            {
                Frontend.instance.Content.Unload();
                // Load default content.
                GlobalContent.LoadDefaultContent(Frontend.instance.Content, Frontend.instance.GraphicsDevice);
                // Load all screen content.
                ScreenManager.LoadContent(Frontend.instance.Content, Frontend.instance.GraphicsDevice);
            }
            GlobalContent.PlaySound("Start");
        }
        public static Color GetColor(string colorName)
        {
            Color color = activeTheme.GetColor(colorName);
            if (color == Color.Transparent)
            {
                color = DefaultThemes.Nonsensical.GetColor(colorName);
            }
            return color;
        }
    }
}
