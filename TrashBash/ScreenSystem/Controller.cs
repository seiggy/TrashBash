using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TrashBash.ScreenSystem
{
    public struct Controller
    {
        /// <summary>
        /// Keys
        /// </summary>
        public Keys keyUp;
        public Keys keyDown;
        public Keys keyLeft;
        public Keys keyRight;
        public Keys keyAction;
        public Keys keyBack;
        public Keys keyPause;
        public Keys keyThrust;
        public Keys keyReverse;
        public Keys keyTractor;
        public Keys keyWeaponA;
        public Keys keyWeaponB;
        public Keys keyWeaponC;

        /// <summary>
        /// game pad buttons
        /// </summary>
        public Buttons joyMove;
        public Buttons joyTriggerThrust;
        public Buttons joyTriggerReverse;
        public Buttons joyAction;
        public Buttons joyBack;
        public Buttons joyPause;
        public Buttons joyTractor;
        public Buttons joyWeaponA;
        public Buttons joyWeaponB;
        public Buttons joyWeaponC;

        /// <summary>
        /// Used to check if game pad is being used
        /// </summary>
        public bool useJoyStick; 

    }
}
