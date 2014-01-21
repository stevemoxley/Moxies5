using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Moxies5.Utilities
{
    public class Timer
    {
        #region Fields
        private float _elapsedTime = -1;
        private bool _running = true;
        private bool _done = false;
        #endregion

        #region Properties

        public float TimeToCount { get; set; }

        public float ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
            set
            {
                if (value >= 0)
                    _elapsedTime = value;
                else
                    value = 0;
            }
        }

        public bool Done
        {
            get
            {
                return _done;
            }
        }

        public bool Running
        {
            get
            {
                return _running;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// A timer that counts down. Will have Property Done set to True when the timer has finished
        /// </summary>
        /// <param name="timeToCount">In seconds</param>
        public Timer(float timeToCount)
        {
            TimeToCount = timeToCount;
            _elapsedTime = timeToCount;
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {
            
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_running)
            {
                if (_elapsedTime > 0)
                {
                    _elapsedTime -= elapsed;
                    _done = false;
                }
                else if (_elapsedTime <= 0)
                {
                    _elapsedTime = TimeToCount;
                    _done = true;
                }
            }
        }

        public void Start()
        {
            _running = true;
        }

        public void Stop()
        {
            _running = false;
        }

        public void Reset()
        {
            _elapsedTime = TimeToCount;
        }
        #endregion
    }
}
