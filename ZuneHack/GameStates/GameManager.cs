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
    public class GameManager
    {
        protected static GameManager instance;
        protected ContentManager contentManager;
        protected Hashtable textures;
        protected SpriteFont font;
        protected Random rnd;

        public bool doQuit = false;
        public float waitBeforeQuit = 0;

        // The queue of gamestates to use
        protected Stack<GameState> gamestates;

        public static GameManager GetInstance()
        {
            if (instance == null) instance = new GameManager(null);
            return instance;
        }

        public GameManager(ContentManager ContentManager)
        {
            gamestates = new Stack<GameState>();
            contentManager = ContentManager;
            textures = new Hashtable();

            rnd = new Random();
        }

        public GameManager Initialize(ContentManager ContentManager)
        {
            contentManager = ContentManager;
            return this;
        }

        public void PushState(GameState newstate)
        {
            gamestates.Push(newstate);
            newstate.Start();
        }

        public void PopState()
        {
            gamestates.Pop().End();
        }

        public Random Random { get { return rnd; } }

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
        /// Called every game tick
        /// </summary>
        public void Update(float timescale)
        {
            // Update the game states
            if (gamestates.Count > 0)
                gamestates.Peek().Update(timescale);

            Input();
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

        public void Input()
        {
            KeyboardState keyState = Keyboard.GetState(PlayerIndex.One);
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            if (gamestates.Count > 0)
                gamestates.Peek().Input(gamepadState, keyState);
        }

        public void Quit(float waitTime)
        {
            doQuit = true;
            waitBeforeQuit = waitTime;
        }
    }
}