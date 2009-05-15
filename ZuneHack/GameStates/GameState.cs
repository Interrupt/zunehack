using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ZuneHack
{
    public class GameState
    {
        protected GameManager manager;

        public GameState(GameManager Manager)
        {
            manager = Manager;
        }

        public virtual void Start()
        {

        }

        public virtual void End()
        {

        }

        public virtual void Update(float timescale)
        {

        }

        public virtual void Draw(SpriteBatch sprites)
        {

        }

        public virtual void Input(GamePadState gamepadState, KeyboardState keyState)
        {

        }
    }
}
