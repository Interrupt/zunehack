using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    public class NpcActor : Actor
    {
        protected bool isAggro;

        public NpcActor()
        {
            texture = GameManager.GetInstance().GetTexture(@"Walls\Smudge");
            blockMovement = true;
            isAggro = true;
        }

        public NpcActor(Vector2 startPos, string ActorTexture)
        {
            texture = GameManager.GetInstance().GetTexture(@"Actors\" + ActorTexture);
            pos = startPos;
            displayPos = pos;
            blockMovement = true;
        }

        public override void DoTurn()
        {
            if (isAggro && IsVisible())
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
                        if (attributes.CheckHit())
                        {
                            MeleeAttack(GameManager.GetInstance().Player);
                        }
                        else
                        {
                            GameManager.GetInstance().AddMessage(String.Format("The {0} attacks, but misses.", Name));
                        }
                    }
                }
            }

            base.DoTurn();
        }
    }
}