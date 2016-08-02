using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TrashBash.MonoGame.ScreenSystem
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        public GamePadState P1CurrentGamePadState;
        public GamePadState P2CurrentGamePadState;
        public KeyboardState CurrentKeyboardState;
#if !XBOX
        public MouseState CurrentMouseState;
#endif

        public GamePadState P1LastGamePadState;
        public GamePadState P2LastGamePadState;
        public KeyboardState LastKeyboardState;
#if !XBOX
        public MouseState LastMouseState;
#endif

        public static Controller P1Controller = new Controller();
        public static Controller P2Controller = new Controller();

        /// <summary>
        /// Checks for a "menu up" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(P1Controller.keyUp) || IsNewKeyPress(P2Controller.keyUp) || IsNewButtonPress(Buttons.DPadUp);
            }
        }


        /// <summary>
        /// Checks for a "menu down" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(P1Controller.keyDown) || IsNewKeyPress(P2Controller.keyDown) || IsNewButtonPress(Buttons.DPadDown);
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(P1Controller.keyAction) || IsNewKeyPress(P2Controller.keyAction) || IsNewButtonPress(Buttons.A);
            }
        }


        /// <summary>
        /// Checks for a "menu cancel" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(P1Controller.keyBack) || IsNewKeyPress(P2Controller.keyBack) || IsNewButtonPress(Buttons.A);
            }
        }


        /// <summary>
        /// Checks for a "pause the game" input action (on either keyboard or gamepad).
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewKeyPress(P1Controller.keyPause) || IsNewKeyPress(P2Controller.keyPause);
            }
        }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            P1LastGamePadState = P1CurrentGamePadState;
            P2LastGamePadState = P2CurrentGamePadState;
#if !XBOX
            LastMouseState = CurrentMouseState;
#endif
            CurrentKeyboardState = Keyboard.GetState();
            P1CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            P2CurrentGamePadState = GamePad.GetState(PlayerIndex.Two);

#if !XBOX
            CurrentMouseState = Mouse.GetState();
#endif
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        private bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) &&
                    LastKeyboardState.IsKeyUp(key));
        }

        private bool IsNewButtonPress(Buttons button)
        {
            return ((P1CurrentGamePadState.IsButtonDown(button) &&
                P1LastGamePadState.IsButtonUp(button)) ||
                (P2CurrentGamePadState.IsButtonDown(button) &&
                P2LastGamePadState.IsButtonUp(button)));
        }
    }
}