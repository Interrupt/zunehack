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
    public class InputStates
    {
        protected GamePadState curPadState;
        protected GamePadState prvPadState;

        protected KeyboardState curKeyState;
        protected KeyboardState prvKeyState;

        public void Tick()
        {
            prvPadState = curPadState;
            prvKeyState = curKeyState;

            curPadState = GamePad.GetState(PlayerIndex.One);
            curKeyState = Keyboard.GetState(PlayerIndex.One);

            if (prvPadState == null && prvKeyState == null)
            {
                prvPadState = curPadState;
                prvKeyState = curKeyState;
            }
        }

        public bool IsButtonPressed(Buttons btn)
        {
            return curPadState.IsButtonDown(btn);
        }

        public bool IsNewButtonPress(Buttons btn)
        {
            return curPadState.IsButtonDown(btn) && !prvPadState.IsButtonDown(btn);
        }

        public bool IsKeyPressed(Keys key)
        {
            return curKeyState.IsKeyDown(key);
        }

        public bool IsNewKeyPress(Keys key)
        {
            return curKeyState.IsKeyDown(key) && !prvKeyState.IsKeyDown(key);
        }
    }

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

        InputStates input_States;

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
            input_States = new InputStates();
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
            //Update the input states
            input_States.Tick();

            // Update the game states
            if (gamestates.Count > 0)
            {
                gamestates.Peek().Update(timescale);
                gamestates.Peek().Input(input_States);
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

        public void Quit(float waitTime)
        {
            doQuit = true;
            waitBeforeQuit = waitTime;
        }
    }
}