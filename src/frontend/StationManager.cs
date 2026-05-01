using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fleasociety
{
    public static class StationManager
    {
        public static List<IStation> stations = new List<IStation>();
        public static int activeStationIndex = 0;
        public static void LoadStations()
        {
            // Clear existing stations.
            stations.Clear();
            // Load every station in the assembly.
            Type stationType = typeof(IStation);
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
                }).Where(p => stationType.IsAssignableFrom(p) && p.IsClass).ToArray();
            foreach (Type type in types)
            {
                // Add the station to the list.
                IStation? station = (IStation?)Activator.CreateInstance(type);
                if(station != null)
                {
                    // Set stations.
                    stations.Add(station);
                }
            }
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            for(int i = 0; i < stations.Count; i++)
            {
                stations[i].LoadContent(contentManager, graphicsDevice);
            }
        }
        public static void SetActiveStation(string name)
        {
            // Find the station with the matching name.
            activeStationIndex = -1;
            for(int i = 0; i < stations.Count; i++)
            {
                if(stations[i].title == name)
                {
                    activeStationIndex = i;
                    break;
                }
            }
            if(activeStationIndex == -1)
            {
                ConsoleOutput.WriteLine("Station not found: " + name, Color.Red);
                return;
            }
        }
        public static IStation? GetActiveStation()
        {
            IStation? station = stations[activeStationIndex];
            if(station == null)
            {
                ConsoleOutput.WriteLine("No active station!", Color.Red);
            }
            return station;
        }
        public static T? GetStation<T>(string name) where T : IStation
        {
            // Find the station with the matching name.
            IStation? station = null;
            for(int i = 0; i < stations.Count; i++)
            {
                if(stations[i].title == name)
                {
                    station = stations[i];
                    break;
                }
            }
            if(station == null)
            {
                ConsoleOutput.WriteLine("Station not found: " + name, Color.Red);
                return default;
            }
            return (T)station;
        }
        public static bool Update(GameTime gameTime, bool handleInput)
        {
            // Update the currently active station.
            IStation? activeStation = stations[activeStationIndex];
            if(activeStation != null)
            {
                return activeStation.Update(gameTime, handleInput);
            }
            return handleInput;
        }
        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the currently active station.
            IStation? activeStation = stations[activeStationIndex];
            if(activeStation != null)
            {
                activeStation.Draw(gameTime, spriteBatch);
            }
        }
    }
}
