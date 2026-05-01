using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fleasociety
{
    public class AspectRatio
    {
        public int width;
        public int height;
        public Vector2 drawOffset;
        public Point preferredResolution;
        public AspectRatio()
        {
            width = 4;
            height = 3;
            drawOffset = new Vector2(0, 0);
            preferredResolution = new Point(320, 240);
        }
        public AspectRatio(int width, int height, Vector2 drawOffset, Point preferredResolution)
        {
            this.width = width;
            this.height = height;
            this.drawOffset = drawOffset;
            this.preferredResolution = preferredResolution;
        }
        public readonly static List<AspectRatio> All = new()
        {
            // Television
            new AspectRatio(4, 3, new Vector2(0, 0), new Point(320, 240)), // Standard Television
            new AspectRatio(5, 4, new Vector2(0, 8), new Point(320, 256)), // Standard Monitor
            // Mobile
            //new AspectRatio(9, 16, new Vector2(0, 164), new Point(320, 569)),
            //new AspectRatio(9, 20, new Vector2(0, 235), new Point(320, 720)), // Pixel 7a
            // Tablet
            new AspectRatio(3, 2, new Vector2(21, 0), new Point(360, 240)), // Microsoft Surface
            // Widescreen
            new AspectRatio(16, 9, new Vector2(53, 0), new Point(427, 240)), // Widescreen Television
            new AspectRatio(16, 10, new Vector2(32, 0), new Point(384, 240)), // Widescreen Monitor
            // Ultrawide
            new AspectRatio(64, 27, new Vector2(124, 0), new Point(569, 240)), // Consumer ultrawide (21:9)
            //new AspectRatio(32, 9, new Vector2(266, 0), new Point(853, 240)), // Super ultrawide
            // Square
            new AspectRatio(1, 1, new Vector2(0, 40), new Point(320, 320)), // Square
        };
    }
    /// <summary>
    /// This class stores repeated graphical functions for ease of use and cleanliness.
    /// </summary>
    public static class GlobalGraphics
    {
        public static float scale = float.Parse(SaveData.saveValues["ScreenScale"], CultureInfo.InvariantCulture);
        public static Vector2 drawOffset = new(0, 0);
        public static Point preferredResolution = new(320, 240);
        public static int scaledWidth = (int)(new AspectRatio().preferredResolution.X * scale);
        public static int scaledHeight = (int)(new AspectRatio().preferredResolution.Y * scale);
        public static AspectRatio FindMatchingAspectRatio()
        {
            AspectRatio closestMatch = new AspectRatio();
            // Get the current aspect ratio of the current screen
            int curWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int curHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            float curAspectRatio = (float)curWidth / curHeight;
            // Get the closest match in AspectRatio.All (aspectRatio.width / aspectRatio.height)
            float closestMatchDifference = Math.Abs(curAspectRatio - (float)closestMatch.width / closestMatch.height);
            foreach(AspectRatio aspectRatio in AspectRatio.All)
            {
                float difference = Math.Abs(curAspectRatio - (float)aspectRatio.width / aspectRatio.height);
                if(difference < closestMatchDifference)
                {
                    closestMatch = aspectRatio;
                    closestMatchDifference = difference;
                }
            }
            // Set the aspect ratio to the closest match
            return new AspectRatio(closestMatch.width, closestMatch.height, closestMatch.drawOffset, new Point(closestMatch.preferredResolution.X, closestMatch.preferredResolution.Y));
        }
        public static void SetAspectRatio(AspectRatio aspectRatio)
        {
            drawOffset = aspectRatio.drawOffset;
            preferredResolution = new Point((int)(aspectRatio.preferredResolution.X * scale), (int)(aspectRatio.preferredResolution.Y * scale));
            if(Frontend.instance != null)
            {
                Frontend.instance.Resize(preferredResolution.X, preferredResolution.Y);
            }
        }
        public static AspectRatio GetAspectRatio()
        {
            foreach (AspectRatio aspectRatio in AspectRatio.All)
            {
                if (aspectRatio.drawOffset == drawOffset && aspectRatio.preferredResolution == new Point((int)(preferredResolution.X / scale), (int)(preferredResolution.Y / scale)))
                    return aspectRatio;
            }
            return new AspectRatio();
        }
        public static int Scale(int value)
        {
            return (int)(value * scale);
        }
        public static float Scale(float value)
        {
            return (float)(value * scale);
        }
        public static Point Scale(Point value)
        {
            return new Point(Scale(value.X), Scale(value.Y));
        }
        public static Rectangle Scale(Rectangle value)
        {
            return new Rectangle(Scale(value.Location), Scale(value.Size));
        }
        public static void DrawString(SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color = default)
        {
            if (color == default)
                color = ThemeManager.GetColor("Text");
            // Offset text if font has an offset.
            position += GlobalContent.GetFontOffset(spriteFont);
            spriteBatch.DrawString(spriteFont, text, position, color);
        }
        public static void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color = default, Color shadowColor = default)
        {
            if (shadowColor == default)
                shadowColor = ThemeManager.GetColor("TextShadow");
            DrawString(spriteBatch, spriteFont, text, position + new Vector2(1, 1), shadowColor);
            DrawString(spriteBatch, spriteFont, text, position, color);
        }
        public static Rectangle DrawButton(SpriteBatch spriteBatch, int x, int y, string text)
        {
            Vector2 measured = L.FontSmall().MeasureString(text);
            // Offset measurements.
            measured.X += Scale(3);
            measured.Y -= Scale(5);
            Rectangle generatedRectangle = new Rectangle(x+Scale(1), y+Scale(1), (int)measured.X, (int)measured.Y);
            spriteBatch.Draw(GlobalContent.GetTexture("Pixel"), generatedRectangle, Color.Black);
            generatedRectangle = new Rectangle(x, y, (int)measured.X, (int)measured.Y);
            spriteBatch.Draw(GlobalContent.GetTexture("Pixel"), generatedRectangle, Color.Gray);
            DrawShadowedString(spriteBatch, L.FontSmall(), text, new Vector2(x+Scale(2), y-Scale(4)));
            return new Rectangle(x, y, (int)measured.X, (int)measured.Y);
        }
    }
}
