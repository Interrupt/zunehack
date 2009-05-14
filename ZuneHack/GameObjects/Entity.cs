using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    public class Entity
    {
        public Vector2 pos;
        public Vector2 displayPos;
        public Vector2 dir = new Vector2(0, 1);
        public Texture2D texture;
        public bool blockMovement = false;
        public bool directional;

        protected Map ownerMap;

        public Entity()
        {

        }

        public Entity(Vector2 position, string tex, bool BlockMovement)
        {
            pos = position;
            displayPos = pos;
            texture = GameManager.GetInstance().GetTexture(tex);
            blockMovement = BlockMovement;
        }

        public Entity(Vector2 position, string tex, bool BlockMovement, bool Directional)
        {
            pos = position;
            displayPos = pos;
            texture = GameManager.GetInstance().GetTexture(tex);
            blockMovement = BlockMovement;
            directional = Directional;
        }

        // Easy ways to find which tile of the map the entity is on
        public int MapPosX { get { return (int)pos.X; } }
        public int MapPosY { get { return (int)pos.Y; } }

        /// <summary>
        /// Initializes the entity on the map
        /// </summary>
        public void SetMap(Map map)
        {
            ownerMap = map;
        }

        /// <summary>
        /// Gets called every game update, used for animation and logic
        /// </summary>
        public virtual void Update(float timescale)
        {
        }

        /// <summary>
        /// Initializes the entity, occurs when the entity is added to a map
        /// </summary>
        public virtual void Initialize()
        {
            displayPos = pos;
        }

        /// <summary>
        /// Check to see if this monster is visible. For now is just checking the distance from the player
        /// </summary>
        public bool IsVisible()
        {
            Vector2 playerPos = ownerMap.Player.pos;
            if (Math.Abs(playerPos.X - pos.X) > 15 || Math.Abs(playerPos.Y - pos.Y) > 15) return false;
            return true;
        }
    }
}