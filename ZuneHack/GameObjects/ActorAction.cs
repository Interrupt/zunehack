using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZuneHack
{
    public abstract class Action
    {
        protected bool isDone;

        protected void Done()
        {
            isDone = true;
        }

        public bool IsDone()
        {
            return isDone;
        }

        public virtual void Update(float timescale)
        {

        }
    }

    class MoveAction : Action
    {
        Vector2 startPos;
        Vector2 endPos;
        Entity entity;
        float donePercentage;
        float speed;

        public MoveAction(float Speed, Vector2 EndPos, Entity Entity)
        {
            entity = Entity;
            speed = Speed;
            startPos = entity.pos;
            endPos = EndPos;
            donePercentage = 0;

            entity.pos = EndPos;
        }

        public override void Update(float timescale)
        {
            donePercentage += speed * timescale;
            if (donePercentage > 1) donePercentage = 1;

            Vector2 move = endPos - startPos;
            entity.displayPos = startPos + (move * donePercentage);

            if (donePercentage >= 1)
            {
                Done();
            }
        }
    }
}