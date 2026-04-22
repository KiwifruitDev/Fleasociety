using Microsoft.Xna.Framework;

namespace KMGEngine
{
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
                if(UserInterface.instance != null)
                    UserInterface.instance.Window.Title = Global.productName+" v"+Global.productVersion;
                debugModePermanent = true;
            }
            debugMode = enable;
        }
        public static bool GetDebugMode()
        {
            return debugMode;
        }
    }
}
