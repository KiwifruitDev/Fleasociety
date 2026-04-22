using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace KMGEngine
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Launch options
            for(int i = 0; i < args.Length; i++)
            {
                Global.parameters.Add(args[i]);
            }
            if(Global.parameters.Contains("-console"))
            {
                string cwd = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".");
                string consoleTxt = Path.Combine(cwd, "console.txt");
                if(File.Exists(consoleTxt))
                {
                    ProcessStartInfo startInfo = new()
                    {
                        FileName = consoleTxt,
                        UseShellExecute = true
                    };
                    ConsoleOutput.WriteLine($"> {startInfo.FileName} {startInfo.Arguments}", Color.Transparent);
                    Process.Start(startInfo);
                    Environment.Exit(0);
                }
                else
                {
                    ConsoleOutput.Clear();
                }
            }
            else if(!Global.parameters.Contains("-keeplog"))
            {
                ConsoleOutput.Clear();
            }
            SaveData.Load();
            if(Global.parameters.Contains("-v"))
            {
                SaveData.saveValues["HiddenVerbose"] = "true";
                SaveData.Save();
            }
            if(Global.parameters.Contains("-scale"))
            {
                int index = Global.parameters.IndexOf("-scale");
                if(index + 1 < Global.parameters.Count)
                {
                    if (float.TryParse(Global.parameters[index + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out float scale))
                    {
                        SaveData.saveValues["ScreenScale"] = scale.ToString();
                    }
                }
            }
            if(Global.parameters.Count > 0)
                ConsoleOutput.WriteLine("Using command line parameters: " + String.Join(" ", Global.parameters.ToArray()));
            if(Global.parameters.Contains("-locale"))
            {
                int index = Global.parameters.IndexOf("-locale");
                if(index + 1 < Global.parameters.Count)
                {
                    SaveData.saveValues["Locale"] = Global.parameters[index + 1];
                }
            }
            // Get language that Steam reports
            bool languageSet = false;
            if(SaveData.saveValues["Locale"] != "fixme")
                languageSet = true;
            // Default to English
            if(!languageSet)
                SaveData.saveValues["Locale"] = "english";
            if(Global.parameters.Contains("-fullscreen"))
                SaveData.saveValues["Fullscreen"] = "true";
            Global.productVersion = (Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0");
            using (var game = new UserInterface())
                game.Run();
            // On graceful exit, save the game data
            SaveData.Save();
        }
    }
}
