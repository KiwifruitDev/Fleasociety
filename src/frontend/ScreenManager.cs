using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KMGEngine
{
    public static class ScreenManager
    {
        public static List<IScreen> drawnScreens = new List<IScreen>();
        public static void LoadScreens()
        {
            // Clear existing screens.
            drawnScreens.Clear();
            // Load every screen in the assembly.
            Type screenType = typeof(IScreen);
            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => {
                    Type[] t;
                    try
                    {
                        t = s.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        // Log or inspect the loader exceptions for debugging
                        foreach (var loaderException in ex.LoaderExceptions)
                        {
                            if (loaderException != null)
                            {
                                Console.WriteLine(loaderException.Message);
                            }
                        }
                        t = ex.Types.Where(type => type != null).Select(type => type!).ToArray(); // Use only successfully loaded types
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading types from assembly: {ex.Message}");
                        t = new Type[0];
                    }
                    return t;
                }).Where(p => screenType.IsAssignableFrom(p) && p.IsClass).ToArray();
            foreach (Type type in types)
            {
                // Add the screen to the list.
                IScreen? screen = (IScreen?)Activator.CreateInstance(type);
                if(screen != null)
                {
                    // Set drawnScreens.
                    drawnScreens.Add(screen);
                }
            }
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            for(int i = 0; i < drawnScreens.Count; i++)
            {
                drawnScreens[i].LoadContent(contentManager, graphicsDevice);
            }
            GlobalContent.AddTexture("Cursor", ThemeManager.LoadLayeredContent<Texture2D>("graphics/cursor"));
        }
        public static void PushNavigation(string name)
        {
            // Find the screen with the matching name.
            IScreen? screen = null;
            for(int i = 0; i < drawnScreens.Count; i++)
            {
                if(drawnScreens[i].title == name)
                {
                    screen = drawnScreens[i];
                    break;
                }
            }
            if(screen == null)
            {
                ConsoleOutput.WriteLine("Screen not found: " + name, Color.Red);
                return;
            }
        }
        public static T? GetScreen<T>(string name) where T : IScreen
        {
            // Find the screen with the matching name.
            IScreen? screen = null;
            for(int i = 0; i < drawnScreens.Count; i++)
            {
                if(drawnScreens[i].title == name)
                {
                    screen = drawnScreens[i];
                    break;
                }
            }
            if(screen == null)
            {
                ConsoleOutput.WriteLine("Screen not found: " + name, Color.Red);
                return default;
            }
            return (T)screen;
        }
        public static void Update(GameTime gameTime)
        {
            if(Debug.frame)
                Debug.paused = false;
            // Handle mouse input, so that screens don't have to do that.
            // Only change if the window is active and the mouse is over the window.
            if(UserInterface.instance != null)
            {
                Input.LastMouseState = Input.MouseState;
                Input.LastKeyboardState = Input.KeyboardState;
                Input._mouseState = Mouse.GetState();
                Input._keyboardState = Keyboard.GetState();
            }
            bool handleInput = UserInterface.instance != null && UserInterface.instance.IsActive && Input.MouseState.X >= 0 && Input.MouseState.X <= GlobalGraphics.scaledWidth &&
                Input.MouseState.Y >= 0 && Input.MouseState.Y <= GlobalGraphics.scaledHeight;
            // Update the drawn screens in layer order and reversed.
            List<IScreen> orderedScreens = drawnScreens.OrderBy(s => s.layer).ToList();
            orderedScreens.Reverse();
            if(!Debug.paused)
            {
                for(int i = 0; i < orderedScreens.Count; i++)
                {
                    if(orderedScreens[i].Update(gameTime, handleInput))
                    {
                        handleInput = false;
                    }
                }
            }
            if(UserInterface.instance != null)
            {
                // F11 or Alt+Enter will toggle fullscreen
                if(Input.KeyboardState.IsKeyDown(Keys.F11) && Input.LastKeyboardState.IsKeyUp(Keys.F11)
                    || Input.KeyboardState.IsKeyDown(Keys.LeftAlt) && Input.KeyboardState.IsKeyDown(Keys.Enter) && Input.LastKeyboardState.IsKeyUp(Keys.Enter))
                {
                    UserInterface.instance.ToggleFullscreen();
                }
                // Alt+F4 will close the game
                if(Input.KeyboardState.IsKeyDown(Keys.LeftAlt) && Input.KeyboardState.IsKeyDown(Keys.F4))
                {
                    UserInterface.instance.ExitGracefully();
                }
            }
            // Toggle debug mode
            // CTRL+F3 will toggle debug mode
            if(Input.KeyboardState.IsKeyDown(Keys.LeftControl) && Input.KeyboardState.IsKeyDown(Keys.F3)
                && Input.LastKeyboardState.IsKeyUp(Keys.F3))
            {
                Debug.SetDebugMode(!Debug.GetDebugMode());
            }
            // DEBUG
            if(Debug.GetDebugMode())
            {
                // F6 will pause
                if(Input.KeyboardState.IsKeyDown(Keys.F6) && Input.LastKeyboardState.IsKeyUp(Keys.F6))
                {
                    Debug.paused = !Debug.paused;
                }
                // F7 will advance a frame
                if(Input.KeyboardState.IsKeyDown(Keys.F7) && Input.LastKeyboardState.IsKeyUp(Keys.F7))
                {
                    Debug.frame = true;
                }
                // F8 will call update 2x
                if(Input.KeyboardState.IsKeyDown(Keys.F8) && !Debug.debugSpeedDebounce)
                {
                    Debug.debugSpeedDebounce = true;
                    for(int i = 0; i < Debug.debugSpeedBoost-1; i++)
                    {
                        Update(gameTime);
                    }
                    Debug.debugSpeedDebounce = false;
                }
                // F9 will reload locales
                if(Input.KeyboardState.IsKeyDown(Keys.F9) && Input.LastKeyboardState.IsKeyUp(Keys.F9))
                {
                    L.ReloadLocales();
                }
                // F10 will unload all locales
                if(Input.KeyboardState.IsKeyDown(Keys.F10) && Input.LastKeyboardState.IsKeyUp(Keys.F10))
                {
                    L.UnloadLocales();
                }
            }
            if(!Debug.paused && Debug.frame)
            {
                Debug.frame = false;
                Debug.paused = true;
            }
        }
        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the screens in layer order.
            List<IScreen> orderedScreens = drawnScreens.OrderBy(s => s.layer).ToList();
            for(int i = 0; i < orderedScreens.Count; i++)
            {
                orderedScreens[i].Draw(gameTime, spriteBatch);
            }
            if(UserInterface.instance != null && !UserInterface.instance.IsMouseVisible)
                spriteBatch.Draw(GlobalContent.GetTexture("Cursor"), new Rectangle(Input.MouseState.Position.X - GlobalGraphics.Scale(4), Input.MouseState.Position.Y - GlobalGraphics.Scale(4), GlobalGraphics.Scale(16), GlobalGraphics.Scale(16)), Color.White);
        }
    }
}
