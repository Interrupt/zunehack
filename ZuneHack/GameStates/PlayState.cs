using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ZuneHack
{
    public class PlayState : GameState
    {
        protected Player player;
        protected Camera cam;
        protected Map map;
        protected Raycaster raycaster;

        public PlayState(GameManager manager) : base(manager)
        {
            cam = new Camera();
            cam.Turn((float)(Math.PI * 2) / 4.0f);

            player = new Player(cam);

            raycaster = new Raycaster(cam, -0.35f);
            raycaster.BackgroundGradient = manager.GetTexture("background-gradient");

            map = new Map(1, MapType.dungeon, this);
            raycaster.SetMap(map);

            Vector2 startLoc = map.GetStairUpLoc();
            cam.SetPosition(startLoc);
        }

        // Returns the current player
        public Player Player { get { return player; } }

        // Runs each entities turn
        public void UpdateTurn()
        {
            for (int i = 0; i < map.entities.Count; i++)
            {
                if (map.entities[i] as Actor != null)
                {
                    ((Actor)map.entities[i]).DoTurn();
                }
            }

            player.StartTurn();
        }

        public void GoDownLevel()
        {
            // Unload the old map
            map.entities.Clear();

            // Load a new map
            map = new Map(map.level + 1, MapType.dungeon, this);

            // Let everything know about the new map
            cam.SetPosition(map.GetStairUpLoc());
            raycaster.SetMap(map);
        }

        public override void Update(float timescale)
        {
            if (player.IsTurnDone())
            {
                UpdateTurn();
            }

            player.pos = cam.pos;
            player.dir = cam.dir;
            player.Update(timescale);

            for (int i = 0; i < map.entities.Count; i++)
            {
                if (player.Stats.curHealth > 0)
                    map.entities[i].Update(timescale);
            }

            raycaster.Update();
        }

        public override void Draw(SpriteBatch batch)
        {
            raycaster.Draw(batch);

            batch.DrawString(manager.Font, String.Format("HP: {0}/{1} MP: {2}/{3}",
                player.Stats.curHealth,
                player.Stats.maxHealth,
                player.Stats.curMana,
                player.Stats.maxMana), new Vector2(2, 2), Color.White);

            if (manager.messages != "")
                batch.DrawString(manager.Font, manager.messages, new Vector2(2, 240 - (19 * manager.numMessages)), Color.White);
        }

        public override void Input(float timescale)
        {
            float rotSpeed = 0.4f * timescale;
            float moveSpeed = 0.4f * timescale;

            KeyboardState keyState = Keyboard.GetState(PlayerIndex.One);
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            if (player.IsActionDone())
            {
                if (gamepadState.DPad.Right == ButtonState.Pressed || keyState.IsKeyDown(Keys.Down))
                {
                    player.TurnInput(PlayerInput.backward);
                }
                else if (gamepadState.DPad.Left == ButtonState.Pressed || keyState.IsKeyDown(Keys.Up))
                {
                    player.TurnInput(PlayerInput.forward);
                }
                else if (gamepadState.DPad.Up == ButtonState.Pressed || keyState.IsKeyDown(Keys.Right))
                {
                    player.TurnInput(PlayerInput.right);
                }
                else if (gamepadState.DPad.Down == ButtonState.Pressed || keyState.IsKeyDown(Keys.Left))
                {
                    player.TurnInput(PlayerInput.left);
                }
                else if (gamepadState.Buttons.A == ButtonState.Pressed || keyState.IsKeyDown(Keys.Space))
                {
                    player.TurnInput(PlayerInput.button);
                }
            }
        }
    }
}