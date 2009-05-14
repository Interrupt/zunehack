using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ZuneHack
{
    class PlayState : GameState
    {
        Camera cam;
        Map map;
        Raycaster raycaster;

        public PlayState(GameManager manager) : base(manager)
        {
            cam = new Camera();
            cam.Turn((float)(Math.PI * 2) / 4.0f);
            manager.Camera = cam;

            raycaster = new Raycaster(cam, -0.35f);
            raycaster.BackgroundGradient = manager.GetTexture("background-gradient");

            map = new Map(1, MapType.dungeon);
            manager.SetMap(map);
            raycaster.SetMap(map);

            cam.SetPosition(map.GetStairUpLoc());
        }

        public override void Update(float timescale)
        {
            raycaster.Update();
        }

        public override void Draw(SpriteBatch batch)
        {
            raycaster.Draw(batch);

            batch.DrawString(manager.Font, String.Format("HP: {0}/{1} MP: {2}/{3}",
                manager.Player.Stats.curHealth,
                manager.Player.Stats.maxHealth,
                manager.Player.Stats.curMana,
                manager.Player.Stats.maxMana), new Vector2(2, 2), Color.White);

            if (manager.messages != "")
                batch.DrawString(manager.Font, manager.messages, new Vector2(2, 240 - (19 * manager.numMessages)), Color.White);
        }

        public override void Input(float timescale)
        {
            float rotSpeed = 0.4f * timescale;
            float moveSpeed = 0.4f * timescale;

            KeyboardState keyState = Keyboard.GetState(PlayerIndex.One);
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            if (manager.Player.IsActionDone())
            {
                if (gamepadState.DPad.Right == ButtonState.Pressed || keyState.IsKeyDown(Keys.Down))
                {
                    manager.Player.TurnInput(PlayerInput.backward);
                }
                else if (gamepadState.DPad.Left == ButtonState.Pressed || keyState.IsKeyDown(Keys.Up))
                {
                    manager.Player.TurnInput(PlayerInput.forward);
                }
                else if (gamepadState.DPad.Up == ButtonState.Pressed || keyState.IsKeyDown(Keys.Right))
                {
                    manager.Player.TurnInput(PlayerInput.right);
                }
                else if (gamepadState.DPad.Down == ButtonState.Pressed || keyState.IsKeyDown(Keys.Left))
                {
                    manager.Player.TurnInput(PlayerInput.left);
                }
                else if (gamepadState.Buttons.A == ButtonState.Pressed || keyState.IsKeyDown(Keys.Space))
                {
                    manager.Player.TurnInput(PlayerInput.button);
                }
            }
        }
    }
}