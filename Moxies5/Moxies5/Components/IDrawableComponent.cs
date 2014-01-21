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

namespace Moxies5
{

    public enum Cameras
    {
        Dynamic,
        Static
    }


    interface IDrawableComponent
    {

        bool Visible { get; }

        int DrawOrder { get; }

        void Draw(GameTime gameTime);

        Cameras Camera { get; }

    }
}
