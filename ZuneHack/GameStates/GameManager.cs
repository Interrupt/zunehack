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
        protected Vector2 playerPos;
        protected Camera camera;
        protected Map map;
        protected Action playerAction = null;
        bool turnDone = false;

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

        /// <summary>
        /// Checks to see if there is an outstanding player action
        /// </summary>
        /// <returns></returns>
        protected bool IsTurnReady()
        {
            return playerAction == null;
        }

        /// <summary>
        /// Loads a texture
        /// </summary>
        /// <param name="texture"></param>
        public Texture2D LoadTexture(string texture)
        {
            textures[texture] = contentManager.Load<Texture2D>(texture);
            return (Texture2D)textures[texture];
        }

        /// <summary>
        /// Returns a texture loaded with LoadTexture
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public Texture2D GetTexture(string texture)
        {
            return (Texture2D)textures[texture];
        }

        /// <summary>
        /// Called every game tick
        /// </summary>
        /// <param name="timescale"></param>
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

        public void Input(float timescale)
        {
            float rotSpeed = 0.4f * timescale;
            float moveSpeed = 0.4f * timescale;

            if (IsTurnReady())
            {
                if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                {
                    if (!map.checkMovability(camera.pos - camera.dir))
                        playerAction = new PlayerMoveAction(0.15f, camera.pos - camera.dir, camera);
                    EndTurn();
                }
                else if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                {
                    if (!map.checkMovability(camera.pos + camera.dir))
                        playerAction = new PlayerMoveAction(0.15f, camera.pos + camera.dir, camera);
                    else
                    {
                        Entity toAttack = map.checkEntityHit(camera.pos + camera.dir);
                        if (toAttack as Actor != null)
                        {
                            playerAction = new PlayerMeleeAction(toAttack);
                        }
                        else if (toAttack as Door != null)
                        {
                            (toAttack as Door).Toggle(0.15f);
                            playerAction = new PlayerPauseAction(0.12f);
                        }
                    }
                    EndTurn();
                }
                else if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                {
                    playerAction = new PlayerTurnAction(0.15f, -1, camera);
                }
                else if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                {
                    playerAction = new PlayerTurnAction(0.15f, 1, camera);
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