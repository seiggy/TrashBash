using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrashBash.MonoGame.ScreenSystem
{
    /// <summary>
    /// Menu to allow player to edit control configuration
    /// </summary>
    internal class EditControlsMenu : MenuScreen
    {
        private enum CurrentController
        {
            PlayerOneKeyboard,
            PlayerOneJoystick,
            PlayerTwoKeyboard,
            PlayerTwoJoystick
        }

        private enum Action
        {
            Up,
            Down,
            Left,
            Right,
            KAction,
            Thrust,
            Reverse,
            WeaponA,
            WeaponB,
            WeaponC,
            Tractor,
            Back,
            Pause
        }

        private enum JoyAction
        {
            Move,
            Thrust,
            Reverse,
            Action,
            WeaponA,
            WeaponB,
            WeaponC,
            Back,
            Pause
        }

        private JoyAction currentJoyAction = JoyAction.Move;

        private Action currentAction = Action.Up;

        private CurrentController currentController = CurrentController.PlayerOneKeyboard;

        private bool waitForInput = false;

        private Buttons[] gamePadButtons =
        {
            Buttons.A,
            Buttons.B,
            Buttons.Back,
            Buttons.DPadDown,
            Buttons.DPadLeft,
            Buttons.DPadRight,
            Buttons.DPadUp,
            Buttons.LeftShoulder,
            Buttons.LeftStick,
            Buttons.LeftThumbstickDown,
            Buttons.LeftThumbstickLeft,
            Buttons.LeftThumbstickRight,
            Buttons.LeftThumbstickUp,
            Buttons.LeftTrigger,
            Buttons.RightShoulder,
            Buttons.RightStick,
            Buttons.RightThumbstickDown,
            Buttons.RightThumbstickLeft,
            Buttons.RightThumbstickRight,
            Buttons.RightThumbstickUp,
            Buttons.RightTrigger,
            Buttons.Start,
            Buttons.X,
            Buttons.Y
        };

        /// <summary>
        /// Constructor to list the controlls
        /// </summary>
        public EditControlsMenu(InputState input)
        {
            switch (currentController)
            {
                case CurrentController.PlayerOneJoystick:
                    MenuEntries.Add("Player 1 - Joystick");
                    break;
                case CurrentController.PlayerOneKeyboard:
                    MenuEntries.Add("Player 1 - Keyboard");
                    break;
                case CurrentController.PlayerTwoJoystick:
                    MenuEntries.Add("Player 2 - Joystick");
                    break;
                case CurrentController.PlayerTwoKeyboard:
                    MenuEntries.Add("Player 2 - Keyboard");
                    break;
                default:
                    break;
            }

            //keyboard for player one
            MenuEntries.Add("Move Up: " + InputState.P1Controller.keyUp.ToString());
            MenuEntries.Add("Move Down: " + InputState.P1Controller.keyDown.ToString());
            MenuEntries.Add("Move Right: " + InputState.P1Controller.keyRight.ToString());
            MenuEntries.Add("Move Left: " + InputState.P1Controller.keyLeft.ToString());
            MenuEntries.Add("Action: " + InputState.P1Controller.keyAction.ToString());
            MenuEntries.Add("Back: " + InputState.P1Controller.keyBack.ToString());
            MenuEntries.Add("Pause: " + InputState.P1Controller.keyPause.ToString());
            MenuEntries.Add("Thrust: " + InputState.P1Controller.keyThrust.ToString());
            MenuEntries.Add("Reverse: " + InputState.P1Controller.keyReverse.ToString());
            MenuEntries.Add("Tractor Beam: " + InputState.P1Controller.keyTractor.ToString());
            MenuEntries.Add("Weapon A: " + InputState.P1Controller.keyWeaponA.ToString());
            MenuEntries.Add("Weapon B: " + InputState.P1Controller.keyWeaponB.ToString());
            MenuEntries.Add("Weapon C: " + InputState.P1Controller.keyWeaponC.ToString());

            //keyboard for player two
            MenuEntries.Add("Move Up: " + InputState.P2Controller.keyUp.ToString());
            MenuEntries.Add("Move Down: " + InputState.P2Controller.keyDown.ToString());
            MenuEntries.Add("Move Right: " + InputState.P2Controller.keyRight.ToString());
            MenuEntries.Add("Move Left: " + InputState.P2Controller.keyLeft.ToString());
            MenuEntries.Add("Action: " + InputState.P2Controller.keyAction.ToString());
            MenuEntries.Add("Back: " + InputState.P2Controller.keyBack.ToString());
            MenuEntries.Add("Pause: " + InputState.P2Controller.keyPause.ToString());
            MenuEntries.Add("Thrust: " + InputState.P2Controller.keyThrust.ToString());
            MenuEntries.Add("Reverse: " + InputState.P2Controller.keyReverse.ToString());
            MenuEntries.Add("Tractor Beam: " + InputState.P2Controller.keyTractor.ToString());
            MenuEntries.Add("Weapon A: " + InputState.P2Controller.keyWeaponA.ToString());
            MenuEntries.Add("Weapon B: " + InputState.P2Controller.keyWeaponB.ToString());
            MenuEntries.Add("Weapon C: " + InputState.P2Controller.keyWeaponC.ToString());

            //JoyStick for player one
            MenuEntries.Add("Movement: " + InputState.P1Controller.joyMove.ToString());
            MenuEntries.Add("Thrust: " + InputState.P1Controller.joyTriggerThrust.ToString());
            MenuEntries.Add("Reverse: " + InputState.P1Controller.joyTriggerReverse.ToString());
            MenuEntries.Add("Action: " + InputState.P1Controller.joyAction.ToString());
            MenuEntries.Add("Weapon A: " + InputState.P1Controller.joyWeaponA.ToString());
            MenuEntries.Add("Weapon B: " + InputState.P1Controller.joyWeaponB.ToString());
            MenuEntries.Add("Weapon c: " + InputState.P1Controller.joyWeaponC.ToString());
            MenuEntries.Add("Tractor Beam: " + InputState.P1Controller.joyTractor.ToString());
            MenuEntries.Add("Back: " + InputState.P1Controller.joyBack.ToString());
            MenuEntries.Add("Pause: " + InputState.P1Controller.joyPause.ToString());

            //joystick for player two
            MenuEntries.Add("Movement: " + InputState.P2Controller.joyMove.ToString());
            MenuEntries.Add("Thrust: " + InputState.P2Controller.joyTriggerThrust.ToString());
            MenuEntries.Add("Reverse: " + InputState.P2Controller.joyTriggerReverse.ToString());
            MenuEntries.Add("Action: " + InputState.P2Controller.joyAction.ToString());
            MenuEntries.Add("Weapon A: " + InputState.P2Controller.joyWeaponA.ToString());
            MenuEntries.Add("Weapon B: " + InputState.P2Controller.joyWeaponB.ToString());
            MenuEntries.Add("Weapon c: " + InputState.P2Controller.joyWeaponC.ToString());
            MenuEntries.Add("Tractor Beam: " + InputState.P2Controller.joyTractor.ToString());
            MenuEntries.Add("Back: " + InputState.P2Controller.joyBack.ToString());
            MenuEntries.Add("Pause: " + InputState.P2Controller.joyPause.ToString());

            LeftBorder = 100;
        }

        /// <summary>
        /// Respond to user selection
        /// </summary>
        /// <param name="entryIndex"></param>
        protected override void OnSelectEntry(int entryIndex)
        {
            switch (entryIndex)
            {
                case 0:
                    waitForInput = true;
                    currentAction = Action.Up;
                    break;
                case 1:
                    waitForInput = true;
                    currentAction = Action.Up;
                    break;
                case 2:
                    waitForInput = true;
                    currentAction = Action.Up;
                    break;
                case 3:
                    waitForInput = true;
                    currentAction = Action.Up;
                    break;
                default:
                    //exit screen
                    ExitScreen();
                    break;
            }
        }

        public override void HandleInput(InputState input)
        {
            if (waitForInput)
            {
                switch (currentController)
                {
                    case CurrentController.PlayerOneKeyboard:
                        DoKeySetup(InputState.P1Controller, input.CurrentKeyboardState);
                        break;
                    case CurrentController.PlayerTwoKeyboard:
                        DoKeySetup(InputState.P2Controller, input.CurrentKeyboardState);
                        break;
                    case CurrentController.PlayerOneJoystick:
                        break;
                    case CurrentController.PlayerTwoJoystick:
                        break;
                }
            }
            base.HandleInput(input);
        }

        /// <summary>
        /// set user key chioce for keyboard controls
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="keystate"></param>
        private void DoKeySetup(Controller controller, KeyboardState keystate)
        {
            switch (currentAction)
            {
                case Action.Up:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyUp = GetKey(keystate);
                    break;
                case Action.Down:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyDown = GetKey(keystate);
                    break;
                case Action.Right:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyRight = GetKey(keystate);
                    break;
                case Action.Left:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyLeft = GetKey(keystate);
                    break;
                case Action.KAction:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyAction = GetKey(keystate);
                    break;
                case Action.Thrust:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyThrust = GetKey(keystate);
                    break;
                case Action.Reverse:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyReverse = GetKey(keystate);
                    break;
                case Action.WeaponA:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyWeaponA = GetKey(keystate);
                    break;
                case Action.WeaponB:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyWeaponB = GetKey(keystate);
                    break;
                case Action.WeaponC:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyWeaponC = GetKey(keystate);
                    break;
                case Action.Back:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyBack = GetKey(keystate);
                    break;
                case Action.Pause:
                    waitForInput = false;
                    if (GetKey(keystate) == Keys.EraseEof)
                        break;
                    controller.keyPause = GetKey(keystate);
                    break;
                default:
                    break;
            }
        }

        private Keys GetKey(KeyboardState current)
        {
            foreach (Keys k in current.GetPressedKeys())
            {
                return k;
            }
            return Keys.EraseEof;
        }

        private void DoJoySetup(Controller controller, GamePadState gamepadState)
        {
            switch (currentJoyAction)
            {
                case JoyAction.Move:
                    waitForInput = false;
                    break;
                case JoyAction.Thrust:
                    waitForInput = false;
                    break;
                case JoyAction.Reverse:
                    waitForInput = false;
                    break;
                case JoyAction.WeaponA:
                    waitForInput = false;
                    break;
                case JoyAction.WeaponB:
                    waitForInput = false;
                    break;
                case JoyAction.WeaponC:
                    waitForInput = false;
                    break;
                case JoyAction.Back:
                    waitForInput = false;
                    break;
                case JoyAction.Pause:
                    waitForInput = false;
                    break;
                case JoyAction.Action:
                    waitForInput = false;
                    break;
            }
        }

        private Buttons GetButton(GamePadState current, GamePadState last)
        {
            foreach (Buttons b in gamePadButtons)
            {
                if (current.IsButtonDown(b))
                {
                    return b;
                }
            }
            return Buttons.BigButton;
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(blendState: BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.DiagnosticSpriteFont,
                                                 "1) Toggle between debug and normal view using either F1 on the keyboard or 'Y' on the controller",
                                                 new Vector2(100, ScreenManager.ScreenHeight - 116), Color.White);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.DiagnosticSpriteFont,
                                                 "2) Keyboard users, use arrows and enter to navigate menus",
                                                 new Vector2(100, ScreenManager.ScreenHeight - 100), Color.White);
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.End();
        }
    }
}
