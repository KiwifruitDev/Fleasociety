using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using MonoGame.Extended.Input;

namespace Fleasociety
{
    public class Frontend : Game
    {
        public static Frontend? instance;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;
        public Frontend()
        {
            ConsoleOutput.WriteLine("Creating new Frontend instance...", Color.Transparent);
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            instance = this;
            if(Global.parameters.Contains("-unlockfps"))
                SetFPSUnlock(true);
        }
        public void SetFPSUnlock(bool unlock)
        {
            IsFixedTimeStep = !unlock;
            _graphics.SynchronizeWithVerticalRetrace = !unlock;
        }
        public void Resize(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
            GlobalGraphics.preferredResolution = new Point(width, height);
        }
        public void SetNativeCursor(bool useNativeCursor)
        {
            IsMouseVisible = useNativeCursor;
        }
        protected override void Initialize()
        {
            ConsoleOutput.WriteLine("Starting initialization for v" + Global.productVersion + "...", Color.Transparent);
#if DEBUG
            Debug.debugBuild = true;
#endif
            Debug.SetDebugMode(Debug.debugBuild || Global.parameters.Contains("-debug"));
            // Disable anti-aliasing.
            _graphics.PreferMultiSampling = false;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            // Set screen resolution.
            if (Global.parameters.Contains("-nosetresolution"))
            {
                ConsoleOutput.WriteLine("Setting screen resolution...", Color.Transparent);
                GlobalGraphics.scale = float.Parse(SaveData.saveValues["ScreenScale"], CultureInfo.InvariantCulture);
                GlobalGraphics.SetAspectRatio(new AspectRatio());
                Resize(GlobalGraphics.scaledWidth, GlobalGraphics.scaledHeight);
                ConsoleOutput.WriteLine("Screen resolution set.", Color.Transparent);
            }
            ScreenManager.LoadScreens();
            StationManager.LoadStations();
            ConsoleOutput.WriteLine("Initialization complete.", Color.Transparent);
            Window.AllowAltF4 = false;
            // match aspect ratio
            AspectRatio aspectRatio = new();
            if (SaveData.saveValues["MatchAspectRatio"] == "true")
                aspectRatio = GlobalGraphics.FindMatchingAspectRatio();
            GlobalGraphics.SetAspectRatio(aspectRatio);
            // hide cursor
            SetNativeCursor(bool.Parse(SaveData.saveValues["UseNativeCursor"]));
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load default content.
            GlobalContent.LoadDefaultContent(Content, GraphicsDevice);
            // Load all screen content.
            ScreenManager.LoadContent(Content, GraphicsDevice);
            base.LoadContent();
        }
        protected override void UnloadContent()
        {
            if(_spriteBatch != null)
                _spriteBatch.Dispose();
            // Unload all content.
            GlobalContent.UnloadContent();
            base.UnloadContent();
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardExtended.Update();
            // Update screens.
            ScreenManager.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ThemeManager.GetColor("ClearColor")); // Black background.
            Global.tooltip = "";
            if(_spriteBatch != null)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null, null, null, Matrix.CreateTranslation(GlobalGraphics.Scale(GlobalGraphics.drawOffset.X), GlobalGraphics.Scale(GlobalGraphics.drawOffset.Y), 0));
                try
                {
                    ScreenManager.Draw(gameTime, _spriteBatch);
                }
                catch(Exception e)
                {
                    ConsoleOutput.WriteLine("Error while drawing screen: " + e.Message, Color.Red);
                }
                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null, null, null, Matrix.CreateTranslation(0, 0, 0));
                // Debug pause indicator
                if(Debug.paused)
                {
                    SpriteFont font = L.FontLarge();
                    string debugPaused = "Debug Paused";
                    Vector2 debugPausedSize = font.MeasureString(debugPaused);
                    GlobalGraphics.DrawShadowedString(_spriteBatch, font, debugPaused, new Vector2(GlobalGraphics.preferredResolution.X-GlobalGraphics.Scale(8)-debugPausedSize.X, GlobalGraphics.Scale(8)), ThemeManager.GetColor("VideoPlayerProgressBar"));
                }
                _spriteBatch.End();
            }
            base.Draw(gameTime);
        }
        public void ExitGracefully()
        {
            if(!Global.exiting)
            {
                Global.exiting = true;
                // Exit page is always the last
                Global.exitOpacityIncrease = 0.0075f;
                Global.fakeExit = true;
                GlobalContent.PlaySound("Quit");
                ConsoleOutput.WriteLine("Exiting gracefully...", Color.Transparent);
            }
        }
        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            if(!Global.exiting && !Global.fakeExit)
            {
                ExitGracefully();
                args.Cancel = true;
                return;
            }
            base.OnExiting(sender, args);
        }
    }
}
