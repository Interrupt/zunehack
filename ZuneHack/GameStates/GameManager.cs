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

        public string messages;
        public int numMessages;

        protected Random rnd;

        public Player Player { get { return player; } }

        public bool doQuit = false;

        // The queue of gamestates to use
        protected Queue<GameState> gamestates;

        public static GameManager GetInstance()
        {
            if (instance == null) instance = new GameManager(null, null, null);
            return instance;
        }

        public GameManager(Camera Camera, Map Map, ContentManager ContentManager)
        {
            gamestates = new Queue<GameState>();
            contentManager = ContentManager;
            camera = Camera;
            map = Map;
            textures = new Hashtable();

            player = new Player(new Vector2(21.5f, 11.5f));
            messages = "";

            AddMessage("The air here is stale and musty.");

            rnd = new Random();
        }

        public GameManager Initialize(ContentManager ContentManager)
        {
            contentManager = ContentManager;

            return this;
        }

        public void PushState(GameState newstate)
        {
            gamestates.Enqueue(newstate);
            newstate.Start();
        }

        public void PopState()
        {
            gamestates.Dequeue().End();
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
            set { camera = value; }
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
            // Update the game states
            if (gamestates.Count > 0)
                gamestates.Peek().Update(timescale);

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
            // Draw the game states
            if(gamestates.Count > 0)
                gamestates.Peek().Draw(batch);
        }

        public void Input(float timescale)
        {
            if (gamestates.Count > 0)
                gamestates.Peek().Input(timescale);
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