using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ZuneHack
{
    /// <summary>
    /// Handles the game state, implements the singleton pattern
    /// </summary>
    class GameManager
    {
        protected static GameManager instance;

        protected ContentManager contentManager;
        protected Hashtable textures;
        protected SpriteFont font;

        protected Player player;
        protected Camera camera;
        protected Map map;

        protected string messages;
        protected int numMessages;

        protected Random rnd;

        public Player Player { get { return player; } }

        public bool doQuit = false;

        public static GameManager GetInstance()
        {
            if (instance == null) instance = new GameManager(null, null, null);
            return instance;
        }

        public GameManager(Camera Camera, Map Map, ContentManager ContentManager)
        {
            contentManager = ContentManager;
            camera = Camera;
            map = Map;
            textures = new Hashtable();

            player = new Player(new Vector2(21.5f, 11.5f));
            messages = "";

            AddMessage("The air here is stale and musty.");

            rnd = new Random();
        }

        public GameManager Initialize(Camera Camera, ContentManager ContentManager)
        {
            contentManager = ContentManager;
            camera = Camera;

            return this;
        }

        public Random Random { get { return rnd; } }

        public void SetMap(Map Map)
        {
            map = Map;
        }

        public Map Map
        {
            get { return map; }
        }

        public Camera Camera
        {
            get { return camera; }
        }

        public SpriteFont Font
        {
            get { return font; }
        }

        /// <summary>
        /// Loads a texture
        /// </summary>
        public Texture2D LoadTexture(string texture)
        {
            textures[texture] = contentManager.Load<Texture2D>(texture);
            return (Texture2D)textures[texture];
        }

        /// <summary>
        /// Loads the game's font
        /// </summary>
        public void LoadFont(string FontName)
        {
            font = contentManager.Load<SpriteFont>(FontName);
        }

        /// <summary>
        /// Returns a texture loaded with LoadTexture
        /// </summary>
        public Texture2D GetTexture(string texture)
        {
            return (Texture2D)textures[texture];
        }

        /// <summary>
        /// Adds a line to the displayed messages
        /// </summary>
        public void AddMessage(string newMessage)
        {
            messages = messages + "\n" + newMessage;
            numMessages = messages.Split('\n').Count();
        }

        /// <summary>
        /// Called every game tick
        /// </summary>
        public void Update(float timescale)
        {
            player.pos = camera.pos;
            player.dir = camera.dir;

            if (player.IsTurnDone())
            {
                UpdateTurn();
            }

            Input(timescale);

            player.Update(timescale);
            for (int i = 0; i < map.entities.Count; i++)
            {
                if(player.Stats.curHealth > 0)
                    map.entities[i].Update(timescale);
            }
        }

        /// <summary>
        /// Draws anything needed for this gamestate
        /// </summary>
        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(font, String.Format("HP: {0}/{1} MP: {2}/{3}", player.Stats.curHealth, player.Stats.maxHealth, player.Stats.curMana, player.Stats.maxMana), new Vector2(2,2), Color.White);
            if(messages != "")
                batch.DrawString(font, messages, new Vector2(2, 240 - (19 * numMessages)), Color.White);
        }

        public void Input(float timescale)
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

        public void didTurnAction()
        {
            messages = "";
        }

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

        public void Quit()
        {
            doQuit = true;
        }

        public void GoDownLevel()
        {
            map = new Map(map.level + 1, MapType.dungeon);
            camera.SetPosition(map.GetStairUpLoc());
        }
    }
}