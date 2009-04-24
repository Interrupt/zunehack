using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    class Entity
    {
        public Vector2 pos;
        public Vector2 displayPos;
        public Vector2 dir = new Vector2(0, 1);
        public Texture2D texture;
        public bool blockMovement = false;
        public bool directional;

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

        public virtual void DoTurn()
        {
            
        }

        public virtual void Update(float timescale)
        {
            displayPos = pos;
        }
    }
}