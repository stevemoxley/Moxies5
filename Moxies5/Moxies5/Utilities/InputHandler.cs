#region Author
// Robb O'Driscoll <ohrodr@gmail.com>
// Very basic Input handling mechanism for the XNA framework.
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Moxies5.Utilities
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputHandler : Microsoft.Xna.Framework.GameComponent
    {
        #region Fields

        /// <summary>
        /// Current keyboard state
        /// </summary>
        static KeyboardState currentState;

        /// <summary>
        /// Last keyboard state
        /// </summary>
        static KeyboardState lastState;

        /// <summary>
        /// Current keyboard state
        /// </summary>
        static MouseState currentMouseState;

        /// <summary>
        /// Last keyboard state
        /// </summary>
        static MouseState lastMouseState;

        static float lastScrollWheel;

        static float currentScrollWheel;

        #endregion

        #region Property Region

        /// <summary>
        /// Returns the current keyboard State
        /// </summary>
        public static KeyboardState KeyboardState
        {
            get { return currentState; }
        }

        /// <summary>
        /// Returns the last known keyboard state
        /// </summary>
        public static KeyboardState LastKeyboardState
        {
            get { return lastState; }
        }

        /// <summary>
        /// Returns the current keyboard State
        /// </summary>
        public static MouseState MouseState
        {
            get { return currentMouseState; }
        }

        /// <summary>
        /// Returns the last known keyboard state
        /// </summary>
        public static MouseState LastMouseState
        {
            get { return lastMouseState; }
        }
        #endregion

        #region Constructor Region

        public InputHandler(Game game)
            : base(game)
        {
            // Store the keyboard state upon initialization
            currentState = Keyboard.GetState();
        }
        #endregion

        #region XNA Framework Methods

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before
        /// starting to run.
        /// </summary>
        public override void Initialize()
        {
            // Blank for now
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // We need to store the old state and get the new state on update.
            lastState = currentState;
            currentState = Keyboard.GetState();

            //Get the mouse states
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            base.Update(gameTime);


        }
        #endregion

        #region General Method Region

        /// <summary>
        /// Cycle the states by setting last to current. This is a bad cleanup.
        /// <todo>Clean this up potentially</todo>
        /// </summary>
        public static void Flush()
        {
            lastState = currentState;
            lastMouseState = currentMouseState;
        }

        #endregion

        #region Keyboard Region
        /// <summary>
        /// Check for releases by comparing the previous state to the current state.
        /// In the event of a key release it will have been down, and currently its up
        /// </summary>
        /// <param name="key">This is the key to check for release</param>
        /// <returns></returns>
        public static bool KeyReleased(Keys key)
        {
            return currentState.IsKeyUp(key) &&
            lastState.IsKeyDown(key);
        }

        /// <summary>
        /// Given a previous key state of up determine if its been pressed
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns></returns>
        public static bool KeyPressed(Keys key)
        {
            return currentState.IsKeyDown(key) &&
                   lastState.IsKeyUp(key);
        }

        /// <summary>
        /// Don't examine last state just check if a key is down
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns></returns>
        public static bool KeyDown(Keys key)
        {
            // check if a key is down regardless of current/past state
            return currentState.IsKeyDown(key);
        }
        #endregion

        #region Mouse Region
        ///<summary>
        ///Check if left mouse is down
        ///</summary>
        public static bool LeftMouseDown()
        {
            return (currentMouseState.LeftButton == ButtonState.Pressed);
        }

        ///<summary>
        ///Check if right mouse is down
        ///</summary>
        public static bool RightMouseDown()
        {
            return (currentMouseState.RightButton == ButtonState.Pressed);
        }

        ///<summary>
        ///Check if left mouse is up
        ///</summary>
        public static bool LeftMouseReleased()
        {
            return (currentMouseState.LeftButton == ButtonState.Released);
        }

        ///<summary>
        ///Check if right mouse is up
        ///</summary>
        public static bool RightMouseReleased()
        {
            return (currentMouseState.RightButton == ButtonState.Released);
        }

        ///<summary>
        ///Check if left mouse is click
        ///</summary>
        public static bool LeftMouseClick()
        {
            return (currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed);
        }

        ///<summary>
        ///Check if Right mouse is click
        ///</summary>
        public static bool RightMouseClick()
        {
            return (currentMouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed);
        }


        #endregion

        #region Scroll Wheel
        public static bool ScrollWheelUp()
        {
            currentScrollWheel = MouseState.ScrollWheelValue;
            if (currentScrollWheel > lastScrollWheel)
            {
                lastScrollWheel = currentScrollWheel;
                return true;
            }
            return false;

        }

        public static bool ScrollWheelDown()
        {
            currentScrollWheel = MouseState.ScrollWheelValue;
            if (currentScrollWheel < lastScrollWheel)
            {
                lastScrollWheel = currentScrollWheel;
                return true;
            }
            return false;

        }
        #endregion


    }
}