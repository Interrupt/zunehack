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
        protected string messages;
        public int numMessages;

        // Initializes the game state
        public PlayState(GameManager manager) : base(manager)
        {
            cam = new Camera();
            cam.Turn((float)(Math.PI * 2) / 4.0f);

            raycaster = new Raycaster(cam, -0.35f);
            raycaster.BackgroundGradient = manager.GetTexture("background-gradient");

            messages = "";
            numMessages = 0;
        }

        // Runs when the game state starts for the first time
        public override void Start()
        {
            player = new Player(cam);

            map = new Map(1, MapType.dungeon, this);
            raycaster.SetMap(map);

            cam.SetPosition(map.GetStairUpLoc());

            AddMessage("You step down into the musty air of the dungeon.");
        }

        // Occurs on each update tick
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

        // Draws the state
        public override void Draw(SpriteBatch batch)
        {
            raycaster.Draw(batch);

            batch.DrawString(manager.Font, String.Format("HP: {0}/{1} MP: {2}/{3}",
                player.Stats.curHealth,
                player.Stats.maxHealth,
                player.Stats.curMana,
                player.Stats.maxMana), new Vector2(2, 2), Color.White);

            if (messages != "")
                batch.DrawString(manager.Font, messages, new Vector2(2, 240 - (19 * numMessages)), Color.White);
        }

        // Takes input for the state
        public override void Input(GamePadState gamepadState, KeyboardState keyState)
        {
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

        // Returns the current player
        public Player Player { get { return player; } }

        // Adds a line to the displayed messages
        public void AddMessage(string newMessage)
        {
            messages = messages + "\n" + newMessage;
            numMessages = messages.Split('\n').Count();
        }

        // Loads the next map in the dungeon
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

        // Resets things for the next turn
        public void didTurnAction()
        {
            messages = "";
        }
    }
}