using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZuneHack
{
    class PlayerPauseAction : Action
    {
        float donePercentage;
        float speed;

        public PlayerPauseAction(float Length)
        {
            speed = Length;
            donePercentage = 0;
        }

        public override void Update(float timescale)
        {
            donePercentage += speed * timescale;
            if (donePercentage > 1) donePercentage = 1;

            if (donePercentage >= 1)
            {
                Done();
            }
        }
    }

    class PlayerMoveAction : Action
    {
        Vector2 startPos;
        Vector2 endPos;
        Camera cam;
        float donePercentage;
        float speed;

        public PlayerMoveAction(float Speed, Vector2 EndPos, Camera Cam)
        {
            speed = Speed;
            cam = Cam;
            startPos = cam.pos;
            endPos = EndPos;
            donePercentage = 0;
        }

        public override void Update(float timescale)
        {
            donePercentage += speed * timescale;
            if (donePercentage > 1) donePercentage = 1;

            Vector2 move = endPos - startPos;
            cam.pos = startPos + (move * donePercentage);

            if (donePercentage >= 1)
            {
                Done();
            }
        }
    }

    class PlayerTurnAction : Action
    {
        float turnSpeed;
        double endRadians;
        float donePercentage;

        Vector2 startDir;
        Vector2 startPlane;
        Camera cam;

        public PlayerTurnAction(float speed, int dir, Camera Cam)
        {
            turnSpeed = speed;
            cam = Cam;
            startDir = cam.dir;
            startPlane = cam.plane;

            donePercentage = 0;
            endRadians = (2.0 * Math.PI) / 4.0; // A rotation of 1/4th of a circle
            endRadians *= dir;
        }

        public override void Update(float timescale)
        {
            donePercentage += turnSpeed * timescale;
            if (donePercentage > 1) donePercentage = 1;

            double newRadians = donePercentage * endRadians;
            cam.dir = startDir;
            cam.plane = startPlane;

            Vector2 oldDir = cam.dir;
            cam.dir.X = (float)(cam.dir.X * Math.Cos(newRadians) - cam.dir.Y * Math.Sin(newRadians));
            cam.dir.Y = (float)(oldDir.X * Math.Sin(newRadians) + cam.dir.Y * Math.Cos(newRadians));

            Vector2 oldPlane = cam.plane;
            cam.plane.X = (float)(cam.plane.X * Math.Cos(newRadians) - cam.plane.Y * Math.Sin(newRadians));
            cam.plane.Y = (float)(oldPlane.X * Math.Sin(newRadians) + cam.plane.Y * Math.Cos(newRadians));

            if (donePercentage >= 1)
            {
                Done();
            }
        }
    }

    class PlayerMeleeAction : Action
    {
        float pause;
        float donePercentage;

        public PlayerMeleeAction(Entity hit)
        {
            pause = 0.4f;
            if (hit != null)
                GameManager.GetInstance().Map.entities.Remove(hit);
        }

        public override void Update(float timescale)
        {
            donePercentage += pause * timescale;
            if (donePercentage >= 1) Done();
        }
    }
}