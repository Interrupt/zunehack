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

        protected Vector2 playerPos;
        protected Camera camera;
        protected Map map;

        protected Action playerAction = null;
        protected bool turnDone = false;
        protected string messages;
        protected int numMessages;

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

            AddMessage("The air here is stale and musty.");
        }

        public GameManager Initialize(Camera Camera, ContentManager ContentManager)
        {
            contentManager = ContentManager;
            camera = Camera;

            return this;
        }

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
        /// Checks to see if there is an outstanding player action
        /// </summary>
        protected bool IsTurnReady()
        {
            return playerAction == null;
        }

        /// <summary>
        /// Adds a line to the displayed messages
        /// </summary>
        public void AddMessage(string newMessage)
        {
            messages = newMessage + "\n" + messages;

            int stripEnd = 0;
            for (int i = 0; i < 3; i++, numMessages = i)
            {
                int thisTime = messages.IndexOf('\n', stripEnd);
                if (thisTime == -1) break;
                stripEnd = thisTime + 1;
            }

            messages = messages.Substring(0, stripEnd);
        }

        /// <summary>
        /// Called every game tick
        /// </summary>
        public void Update(float timescale)
        {
            playerPos = camera.pos;
            Input(timescale);

            if (playerAction != null)
            {
                if (!playerAction.IsDone())
                {
                    playerAction.Update(timescale);
                }
                else
                {
                    playerAction = null;

                    if (turnDone == true)
                        UpdateTurn();
                }
            }

            foreach (Entity entities in map.entities)
            {
                entities.Update(timescale);
            }
        }

        /// <summary>
        /// Draws anything needed for this gamestate
        /// </summary>
        public void Draw(SpriteBatch batch)
        {
            if(messages != "")
                batch.DrawString(font, messages, new Vector2(2, 240 - (16 * numMessages)), Color.White);
        }

        public void Input(float timescale)
        {
            float rotSpeed = 0.4f * timescale;
            float moveSpeed = 0.4f * timescale;

            if (IsTurnReady())
            {
                if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                {
                    if (!map.checkMovability(camera.pos - camera.dir))
                        playerAction = new PlayerMoveAction(0.2f, camera.pos - camera.dir, camera);
                    else
                    {
                        playerAction = new PlayerPauseAction(0.4f);
                        AddMessage("Ouch!");
                    }
                    EndTurn();
                }
                else if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                {
                    if (!map.checkMovability(camera.pos + camera.dir))
                        playerAction = new PlayerMoveAction(0.2f, camera.pos + camera.dir, camera);
                    else
                    {
                        Entity toAttack = map.checkEntityHit(camera.pos + camera.dir);
                        if (toAttack as Actor != null)
                        {
                            playerAction = new PlayerMeleeAction(toAttack);
                        }
                        else if (toAttack as Door != null)
                        {
                            (toAttack as Door).Toggle(0.16f);
                            playerAction = new PlayerPauseAction(0.2f);
                        }
                        else
                        {
                            playerAction = new PlayerPauseAction(0.4f);
                            AddMessage("Ouch!");
                        }
                    }
                    EndTurn();
                }
                else if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                {
                    playerAction = new PlayerTurnAction(0.2f, -1, camera);
                }
                else if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                {
                    playerAction = new PlayerTurnAction(0.2f, 1, camera);
                }
            }
        }

        public void EndTurn()
        {
            turnDone = true;
        }

        public void UpdateTurn()
        {
            foreach (Entity entity in map.entities)
            {
                entity.DoTurn();
            }

            // It's now the player's turn again
            turnDone = false;
        }
    }
}