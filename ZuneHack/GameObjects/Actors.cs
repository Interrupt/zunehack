using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    struct Stats
    {
        public int maxHealth;
        public int strength;
        public int constitution;
        public int dexterity;
        public int speed;
    }

    class Actor : Entity
    {
        protected int health;
        protected Stats stats;
        protected Action action;
        protected bool isAggro;

        public Actor(Vector2 startPos, string ActorTexture)
        {
            texture = GameManager.GetInstance().GetTexture(@"Actors\" + ActorTexture);
            pos = startPos;
            displayPos = pos;
            isAggro = true;
            blockMovement = true;
        }

        public override void DoTurn()
        {
            // Finds the direction to move in that will get closer to the player
            Vector2 playerPos = GameManager.GetInstance().Camera.pos;
            Vector2 newPos = pos;

            int xDist = (int)(pos.X - playerPos.X);
            int yDist = (int)(pos.Y - playerPos.Y);

            int xDir = xDist > 0 ? 1 : -1;
            int yDir = yDist > 0 ? 1 : -1;

            bool canMoveX = !GameManager.GetInstance().Map.checkMovability(pos + new Vector2(-xDir, 0));
            bool canMoveY = !GameManager.GetInstance().Map.checkMovability(pos + new Vector2(0, -yDir));

            if (Math.Abs(xDist) > Math.Abs(yDist))
            {
                if (canMoveX)
                {
                    int dir = xDist > 0 ? 1 : -1;
                    newPos += new Vector2(-dir, 0);
                }
                else if (canMoveY)
                {
                    int dir = yDist > 0 ? 1 : -1;
                    newPos += new Vector2(0, -dir);
                }
            }
            else
            {
                if (canMoveY)
                {
                    int dir = yDist > 0 ? 1 : -1;
                    newPos += new Vector2(0, -dir);
                }
                else if (canMoveX)
                {
                    int dir = xDist > 0 ? 1 : -1;
                    newPos += new Vector2(-dir, 0);
                }
            }

            // Check to see if we can walk here
            if (newPos != pos)
            {
                if (newPos != playerPos)
                {
                    action = new MoveAction(0.2f, newPos, this);
                }
                else
                {
                    GameManager.GetInstance().AddMessage("Monster attacks!");
                }
            }
        }

        public override void Update(float timescale)
        {
            if (action != null)
            {
                if (!action.IsDone())
                {
                    action.Update(timescale);
                }
                else
                {
                    action = null;
                }
            }
        }

        public bool IsTurnDone()
        {
            return (action == null);
        }
    }
}