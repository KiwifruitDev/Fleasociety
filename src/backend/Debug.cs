using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Fleasociety
{
    public class DebugButton
    {
        private Action onClick = () => {};
        private Func<string> getText = () => "";
        private Func<string> getTooltip = () => "";
        public DebugButton(Action onClick, Func<string> getText, Func<string> getTooltip)
        {
            this.onClick = onClick;
            this.getText = getText;
            this.getTooltip = getTooltip;
        }
        public string GetText()
        {
            return getText();
        }
        public string GetTooltip()
        {
            return getTooltip();
        }
        public void Click()
        {
            onClick();
        }
    }
    public static class Debug
    {
        public static bool debugModePermanent = false;
        public static bool debugBuild = false;
        private static bool debugMode = false;
        private static Color debugColor = new Color(255, 128, 64, 255);
        public static bool paused = false;
        public static bool frame = false;
        public static bool debugSpeedDebounce = false;
        public static int debugSpeedBoost = 2;
        private static int soundTest = 0;
        private static List<List<DebugButton>> debugButtonPages = new List<List<DebugButton>>();
        private static DebugButton nextPageButton = new DebugButton(
            () => {
                SetDebugPage(debugPage + 1);
                GlobalContent.PlaySound("Select");
            },
            () => {
                return "Next Page";
            },
            () =>
            {
                return "Go to the next page of debug buttons.";
            }
        );
        private static int debugPage = 0;
        public static void SetDebugPage(int page)
        {
            if(page >= 0 && page < debugButtonPages.Count)
            {
                debugPage = page;
            }
            else
            {
                debugPage = 0;
            }
        }
        public static List<DebugButton> GetDebugButtons()
        {
            if(debugPage >= 0 && debugPage < debugButtonPages.Count)
            {
                return debugButtonPages[debugPage];
            }
            else
            {
                return new List<DebugButton>();
            }
        }
        public static void PopulateDebugButtons()
        {
            debugButtonPages.Clear();
            debugButtonPages.Add(new List<DebugButton>() {
                nextPageButton,
                new DebugButton(
                    () => {
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
                    },
                    () => {
                        return "Locale: " + L.GetLocale().name + " " + L.GetLocale().localizedName;
                    },
                    () =>
                    {
                        return "Change the game's language. Currently: " + L.GetLocale().name;
                    }
                ),
                new DebugButton(
                    () => {
                        if(Frontend.instance != null)
                            Frontend.instance.Window.AllowUserResizing = !Frontend.instance.Window.AllowUserResizing;
                        GlobalContent.PlaySound("Select");
                    },
                    () => {
                        return "User Resizing: " + (Frontend.instance != null && Frontend.instance.Window.AllowUserResizing ? "Enabled" : "Disabled");
                    },
                    () =>
                    {
                        return "Toggle whether the game window can be resized by the user.";
                    }
                ),
                new DebugButton(
                    () => {
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
                    },
                    () => {
                        return "Screen Scale: " + SaveData.saveValues["ScreenScale"];
                    },
                    () =>
                    {
                        return "Change the game's screen scale. Higher values may\nimprove performance but make the game look more pixelated.";
                    }
                ),
                new DebugButton(
                    () => {
                        Debug.debugSpeedBoost++;
                        if(Debug.debugSpeedBoost > 10)
                            Debug.debugSpeedBoost = 2;
                        GlobalContent.PlaySound("Select");
                    },
                    () => {
                        return "Speed Boost: x" + Debug.debugSpeedBoost;
                    },
                    () =>
                    {
                        return "Change how much the game's speed is boosted when debug\nspeed boost is active. Higher values will make the game run faster.";
                    }
                ),
                new DebugButton(
                    () => {
                        AspectRatio current = GlobalGraphics.GetAspectRatio();
                        int index = AspectRatio.All.IndexOf(current);
                        AspectRatio next = AspectRatio.All[(index + 1) % AspectRatio.All.Count];
                        GlobalGraphics.SetAspectRatio(next);
                        GlobalContent.PlaySound("Select");
                    },
                    () => {
                        return "Draw Offset: " + GlobalGraphics.drawOffset.X.ToString() + ", " + GlobalGraphics.drawOffset.Y.ToString();
                    },
                    () =>
                    {
                        return "Change the game's draw offset.\nThis can be used to fix graphical issues on ultrawide monitors.";
                    }
                ),
                new DebugButton(
                    () => {
                        GlobalContent.PlaySound("Select");
                        string name = ThemeManager.activeTheme.name;
                        for(int i = 0; i < DefaultThemes.themes.Count(); i++)
                        {
                            if(name == DefaultThemes.themes[i].name)
                            {
                                if(i + 1 < DefaultThemes.themes.Count())
                                {
                                    ThemeManager.ApplyTheme(DefaultThemes.themes[i + 1]);
                                    return;
                                }
                                break;
                            }
                        }
                        ThemeManager.ApplyTheme(DefaultThemes.themes[0]);
                    },
                    () => {
                        return "Theme: " + ThemeManager.activeTheme.name;
                    },
                    () =>
                    {
                        return "Change the game's theme.\nThis will change the colors and some textures used in the game.";
                    }
                ),
                new DebugButton(
                    () => {
                        SaveData.Save();
                        GlobalContent.PlaySound("Select");
                    },
                    () => {
                        return "Save";
                    },
                    () =>
                    {
                        return "Save the game.";
                    }
                ),
                new DebugButton(
                    () => {
                        GlobalContent.PlaySound("Select");
                        SaveData.saveValues["HiddenVerbose"] = (!bool.Parse(SaveData.saveValues["HiddenVerbose"])).ToString();
                        SaveData.Save();
                    },
                    () => {
                        return (bool.Parse(SaveData.saveValues["HiddenVerbose"]) ? "Disable" : "Enable") + " Verbose";
                    },
                    () =>
                    {
                        return "Toggle whether verbose output is hidden.\nThis will hide some of the more spammy debug messages in the console.";
                    }
                ),
                new DebugButton(
                    () => {
                        GlobalContent.PlaySound("Select");
                        string consoleLogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "console.txt");
                        if(File.Exists(consoleLogFile))
                        {
                            Process.Start("notepad", consoleLogFile);
                        }
                    },
                    () => {
                        return "Open console.txt";
                    },
                    () =>
                    {
                        return "Open the console log file in Notepad.\nThis file contains all of the debug output from the game.";
                    }
                ),
                new DebugButton(
                    () => {
                        GlobalContent.PlaySound("Select");
                        if(Frontend.instance != null)
                            Frontend.instance.SetFPSUnlock(Frontend.instance.IsFixedTimeStep);
                    },
                    () => {
                        return (Frontend.instance != null && Frontend.instance.IsFixedTimeStep ? "Unlock" : "Lock") + " FPS and VSync";
                    },
                    () =>
                    {
                        return "Toggle whether the game's FPS is locked to the monitor's refresh rate.\nUnlocking FPS may improve performance but can cause screen tearing.";
                    }
                ),
                new DebugButton(
                    () => {
                        soundTest++;
                        Dictionary<string, SoundEffect> sounds = GlobalContent.GetSounds();
                        if (sounds.Count > 0)
                        {
                            List<string> keys = sounds.Keys.ToList();
                            if (soundTest >= keys.Count)
                            {
                                soundTest = 0;
                            }
                            string soundName = keys[soundTest % keys.Count];
                            GlobalContent.PlaySound(soundName);
                        }
                    },
                    () => {
                        return "Sound test: " + soundTest.ToString();
                    },
                    () =>
                    {
                        return "Play a sound from the game's sound library.\nThis is useful for testing whether sounds are working correctly.";
                    }
                )
            });
        }
        public static void SetDebugMode(bool enable)
        {
            if(!debugModePermanent)
            {
                if(debugBuild)
                {
                    ConsoleOutput.WriteLine("Debug Build (!!!)", debugColor);
                    Global.productVersion = Global.productVersion+" (DEBUG BUILD)";
                }
                else
                {
                    ConsoleOutput.WriteLine("Debug Mode (!!!)", debugColor);
                    Global.productVersion = Global.productVersion+" (DEBUG MODE)";
                }
                if(Frontend.instance != null)
                    Frontend.instance.Window.Title = Global.productName+" v"+Global.productVersion;
                debugModePermanent = true;
                PopulateDebugButtons();
            }
            debugMode = enable;
        }
        public static bool GetDebugMode()
        {
            return debugMode;
        }
    }
}
