using UnityEngine.InputSystem;

namespace StormWaits
{
    
    
    public static class MinigameInput
    {
        
        public static bool KeyPressedThisFrame(Key key)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || key == Key.None) return false;

            return keyboard[key].wasPressedThisFrame;
        }

        
        public static bool KeyHeld(Key key)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || key == Key.None) return false;

            return keyboard[key].isPressed;
        }

        
        
        private static bool MouseCounts
        {
            get
            {
                return GameSettings.Current.allowMouseClick && !MinigameManager.CursorFree;
            }
        }

        
        public static bool ActionHeld
        {
            get
            {
                Mouse mouse = Mouse.current;
                bool clickHeld = MouseCounts && mouse != null && mouse.leftButton.isPressed;
                return KeyHeld(GameSettings.Current.actionKey) || clickHeld;
            }
        }

        
        public static bool ActionPressed
        {
            get
            {
                Mouse mouse = Mouse.current;
                bool clickPressed = MouseCounts && mouse != null && mouse.leftButton.wasPressedThisFrame;
                return KeyPressedThisFrame(GameSettings.Current.actionKey) || clickPressed;
            }
        }

        
        public static string ActionName
        {
            get
            {
                GameSettings settings = GameSettings.Current;
                string name = settings.actionKey.ToString();
                if (settings.allowMouseClick) name += " or left click";
                return name;
            }
        }
    }
}
