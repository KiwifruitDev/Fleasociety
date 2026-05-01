using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fleasociety
{
    public class MisclickCircle
    {
        public Vector2 circleClick = new Vector2(0, 0);
        public int circleSize = 1;
        public int circleValue = 100;
        public int circleAlpha = 255;
        public int rotation = 0;
        public MisclickCircle(Vector2 circleClick)
        {
            this.circleClick = circleClick;
        }
        public void Update()
        {
            if(circleClick.X != 0 && circleClick.Y != 0)
            {
                rotation += 2;
                circleAlpha -= 16;
                circleValue -= 6;
                circleSize += GlobalGraphics.Scale(2);
                if(circleAlpha <= 0)
                {
                    // By setting the circle size to 0, the circle will be removed from the screen.
                    circleClick = new Vector2(0, 0);
                    circleSize = 0;
                }
            }
        }
    }
    /// <summary>
    /// This is the background screen, it draws a scrolling tiled pattern.
    /// </summary>
    public class MisclickScreen : IScreen
    {
        /// <summary>
        /// The title of the screen. This is displayed on the header bar.
        /// </summary>
        public string title { get; } = "Misclick";
        public int layer { get; set; } = 100;
        private float hueColor = 0;
        private List<MisclickCircle> circles = new List<MisclickCircle>();
        private void HSVToRGB(int h, int s, int v, out Color color)
        {
            var rgb = new int[3];

            var baseColor = (h + 60) % 360 / 120;
            var shift = (h + 60) % 360 - (120 * baseColor + 60 );
            var secondaryColor = (baseColor + (shift >= 0 ? 1 : -1) + 3) % 3;

            //Setting Hue
            rgb[baseColor] = 255;
            rgb[secondaryColor] = (int) ((Math.Abs(shift) / 60.0f) * 255.0f);

            //Setting Saturation
            for (var i = 0; i < 3; i++)
                rgb[i] += (int) ((255 - rgb[i]) * ((100 - s) / 100.0f));

            //Setting Value
            for (var i = 0; i < 3; i++)
                rgb[i] -= (int) (rgb[i] * (100-v) / 100.0f);

            color = new Color(rgb[0], rgb[1], rgb[2]);
        }
        public bool Update(GameTime gameTime, bool handleInput)
        {
            // Change color by hue.
            hueColor += 0.0625f;
            // Wrap to 0 after 360.
            if (hueColor >= 360)
                hueColor = 0;
            // Input.
            if(handleInput)
            {
                // Detect clicks.
                if (Input.LastMouseState.LeftButton == ButtonState.Released && Input.MouseState.LeftButton == ButtonState.Pressed) {
                    // Add a circle.
                    circles.Add(new MisclickCircle(new Vector2(Input.MouseState.X, Input.MouseState.Y)));
                }
            }
            // Draw circles indicating misclicks.
            for(int i = 0; i < circles.Count; i++)
            {
                if(circles[i].circleSize <= 0)
                {
                    circles.RemoveAt(i);
                    i--;
                }
                else
                    circles[i].Update();
            }
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw circles indicating misclicks.
            foreach(MisclickCircle circle in circles)
            {
                HSVToRGB((int)hueColor, 100, circle.circleValue, out Color circleColor);
                circleColor = new Color(circleColor, circle.circleAlpha);
                //GlobalGraphics.DrawCircle(spriteBatch, circle.circleClick, circle.circleSize, circleColor);
                Texture2D circleTexture = GlobalContent.GetTexture("MisclickCircle");
                Rectangle circlePos = new Rectangle((int)circle.circleClick.X, (int)circle.circleClick.Y, circle.circleSize, circle.circleSize);
                spriteBatch.Draw(circleTexture, circlePos, null, circleColor, MathHelper.ToRadians(circle.rotation), new Vector2(circleTexture.Width/2, circleTexture.Height/2), SpriteEffects.None, 0);
            }
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            GlobalContent.AddTexture("MisclickCircle", ThemeManager.LoadLayeredContent<Texture2D>("graphics/misclick"));
        }
    }
}
