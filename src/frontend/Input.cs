using Microsoft.Xna.Framework.Input;

namespace KMGEngine
{ 
    public static class Input
    {
        public static MouseState _mouseState;
        private static MouseState lastMouseState;
        public static MouseState MouseState
        {
            get { return CompatMouseState(); }
            set { }
        }
        public static MouseState CompatMouseState()
        {
            // Respect GlobalGraphics.drawOffset
            int oX = GlobalGraphics.Scale((int)GlobalGraphics.drawOffset.X);
            int oY = GlobalGraphics.Scale((int)GlobalGraphics.drawOffset.Y);
            MouseState overrideLastState = lastMouseState;
            MouseState overrideMouseState = _mouseState;
            // Apply draw offset
            overrideLastState = new MouseState(overrideLastState.X - oX, overrideLastState.Y - oY, overrideLastState.ScrollWheelValue, overrideLastState.LeftButton, overrideLastState.MiddleButton, overrideLastState.RightButton, overrideLastState.XButton1, overrideLastState.XButton2);
            overrideMouseState = new MouseState(overrideMouseState.X - oX, overrideMouseState.Y - oY, overrideMouseState.ScrollWheelValue, overrideMouseState.LeftButton, overrideMouseState.MiddleButton, overrideMouseState.RightButton, overrideMouseState.XButton1, overrideMouseState.XButton2);
            lastMouseState = overrideLastState;
            return overrideMouseState;
        }
        public static MouseState LastMouseState
        {
            get
            {
                return lastMouseState;
            }
            set
            {
                lastMouseState = value;
            }
        }
        public static KeyboardState _keyboardState;
        public static KeyboardState lastKeyboardState;
        public static KeyboardState KeyboardState
        {
            get { return CompatKeyboardState(); }
            set { }
        }
        public static KeyboardState CompatKeyboardState()
        {
            KeyboardState overrideLastState = lastKeyboardState;
            KeyboardState overrideKeyboardState = _keyboardState;
            lastKeyboardState = overrideLastState;
            return overrideKeyboardState;
        }
        public static KeyboardState LastKeyboardState
        {
            get
            {
                return lastKeyboardState;
            }
            set
            {
                lastKeyboardState = value;
            }
        }
    }
}
