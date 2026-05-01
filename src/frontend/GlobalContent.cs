using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Fleasociety
{
    /// <summary>
    /// Store content for access by other classes.
    /// </summary>
    public static class GlobalContent
    {
        private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();
        private static Dictionary<SpriteFont, Vector2> fontOffsets = new Dictionary<SpriteFont, Vector2>();
        private static List<Song> songs = new List<Song>();
        public static void LoadDefaultContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            // Dispose all existing content.
            UnloadContent();
            L.ReloadLocales();
            // Load default sounds.
            AddSound("AddSource", ThemeManager.LoadLayeredContent<SoundEffect>("sound/addsource"));
            AddSound("Back", ThemeManager.LoadLayeredContent<SoundEffect>("sound/back"));
            AddSound("Error", ThemeManager.LoadLayeredContent<SoundEffect>("sound/error"));
            AddSound("Hover", ThemeManager.LoadLayeredContent<SoundEffect>("sound/hover"));
            AddSound("Option", ThemeManager.LoadLayeredContent<SoundEffect>("sound/option"));
            AddSound("Prompt", ThemeManager.LoadLayeredContent<SoundEffect>("sound/prompt"));
            AddSound("Quit", ThemeManager.LoadLayeredContent<SoundEffect>("sound/quit"));
            AddSound("RenderComplete", ThemeManager.LoadLayeredContent<SoundEffect>("sound/rendercomplete"));
            AddSound("Select", ThemeManager.LoadLayeredContent<SoundEffect>("sound/select"));
            AddSound("Start", ThemeManager.LoadLayeredContent<SoundEffect>("sound/start"));
            AddSound("CompatSelect", ThemeManager.LoadLayeredContent<SoundEffect>("sound/compatselect"));
            AddSound("Disambiguation", ThemeManager.LoadLayeredContent<SoundEffect>("sound/disambiguation"));
            // Load default fonts.
            int scale = (int)float.Parse(SaveData.saveValues["ScreenScale"], CultureInfo.InvariantCulture);
            // font scale must be between 1 and 4
            if (scale < 1 || scale > 4)
            {
                scale = 2;
            }
            AddFont("Munro", ThemeManager.LoadLayeredContent<SpriteFont>("fonts/munro-x"+scale), new Vector2(0, 0));
            AddFont("MunroSmall", ThemeManager.LoadLayeredContent<SpriteFont>("fonts/munro-small-x"+scale), new Vector2(0, 0));
            // Create pixel shape.
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData([Color.White]);
            AddTexture("Pixel", pixel);
            // Create a hollow circle.
            int circleSize = GlobalGraphics.Scale(128);
            Texture2D circle = new Texture2D(graphicsDevice, circleSize, circleSize);
            Color[] data = new Color[circleSize * circleSize];
            for(int x = 0; x < circleSize; x++)
            {
                for(int y = 0; y < circleSize; y++)
                {
                    int distance = (int)Math.Sqrt(Math.Pow(x - circleSize/2, 2) + Math.Pow(y - circleSize/2, 2));
                    if(distance < circleSize/2)
                    {
                        data[x + y * circleSize] = Color.White;
                    }   
                    else
                    {
                        data[x + y * circleSize] = Color.Transparent;
                    }
                }
            }
            float hollowInside = GlobalGraphics.scale * 4;
            for(int x = 0; x < circleSize-hollowInside; x++)
            {
                for(int y = 0; y < circleSize-hollowInside; y++)
                {
                    int distance = (int)Math.Sqrt(Math.Pow(x - circleSize/2, 2) + Math.Pow(y - circleSize/2, 2));
                    if(distance < (circleSize/2)-hollowInside)
                    {
                        data[x + y * circleSize] = Color.Transparent;
                    }
                }
            }
            circle.SetData(data);
            AddTexture("HollowCircle", circle);
            // Create a filled circle.
            Texture2D filledCircle = new Texture2D(graphicsDevice, circleSize, circleSize);
            Color[] data2 = new Color[circleSize * circleSize];
            for (int i = 0; i < data2.Length; ++i)
            {
                int x = i % circleSize;
                int y = i / circleSize;
                int distance = (int)Math.Sqrt(x * x + y * y);
                if (distance <= circleSize/2)
                {
                    data2[i] = Color.White;
                }
                else
                {
                    data2[i] = Color.Transparent;
                }
            }
            filledCircle.SetData(data2);
            AddTexture("FilledCircle", filledCircle);
        }
        public static void UnloadContent()
        {
            foreach(SoundEffect sound in sounds.Values)
            {
                sound.Dispose();
            }
            foreach(Texture2D texture in textures.Values)
            {
                texture.Dispose();
            }
            foreach(SpriteFont font in fonts.Values)
            {
                fontOffsets.Remove(font);
            }
            foreach(Song song in songs)
            {
                song.Dispose();
            }
            sounds.Clear();
            textures.Clear();
            fonts.Clear();
            songs.Clear();
        }
        public static Texture2D AddTexture(string name, Texture2D texture)
        {
            if (textures.ContainsKey(name))
            {
                textures[name].Dispose();
                textures[name] = texture;
            }
            else
            {
                textures.Add(name, texture);
            }
            return texture;
        }
        public static bool AddFont(string name, SpriteFont font, Vector2 offset)
        {
            if (fonts.ContainsKey(name))
            {
                fonts[name] = font;
                fontOffsets[font] = offset;
            }
            else
            {
                fonts.Add(name, font);
                fontOffsets.Add(font, offset);
            }
            return true;
        }
        public static bool AddSound(string name, SoundEffect sound)
        {
            if (sounds.ContainsKey(name))
            {
                sounds[name].Dispose();
                sounds[name] = sound;
            }
            else
            {
                sounds.Add(name, sound);
            }
            return true;
        }
        public static SoundEffect GetSound(string name)
        {
            return sounds[name];
        }
        public static Texture2D GetTexture(string name)
        {
            // if texture doesn't exist, return a 1x1 white pixel.
            if (!textures.ContainsKey(name))
                return textures["Pixel"];
            return textures[name];
        }
        public static SpriteFont GetFont(string name)
        {
            return fonts[name];
        }
        public static Vector2 GetFontOffset(SpriteFont font)
        {
            if (fontOffsets.ContainsKey(font))
                return fontOffsets[font];
            return Vector2.Zero;
        }
        public static Song GetSong(int index)
        {
            return songs[index];
        }
        public static bool CheckFont(string name)
        {
            return fonts.ContainsKey(name);
        }
        public static void PlaySound(string name)
        {
            // Play sound if it exists.
            if (sounds.ContainsKey(name))
            {
                float soundEffectVolume = float.Parse(SaveData.saveValues["SoundEffectVolume"], CultureInfo.InvariantCulture);
                // Check if sound effect volume is set to 0.
                if (soundEffectVolume > 0)
                {
                    sounds[name].Play(soundEffectVolume / 100f, 0.0f, 0.0f);
                }
            }
            else
            {
                ConsoleOutput.WriteLine($"Sound {name} not found.", Color.Red);
            }
        }
    }
}
