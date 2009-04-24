using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    class Camera
    {
        public Vector2 pos;
        public Vector2 dir;
        public Vector2 plane;
        protected float fov;

        public Camera()
        {
            pos = new Vector2();
            dir = new Vector2();
            plane = new Vector2();

            fov = 0.66f;    // FOV will be about 75 degrees

            dir.X = -1;
            dir.Y = 0;

            plane.X = 0;
            plane.Y = fov;
        }

        public float GetFOV()
        {
            return (plane.X * dir.Y - dir.X * plane.Y);
        }

        public void SetPosition(Vector2 newPos)
        {
            pos = newPos;
        }

        public void MoveForward(float amount)
        {
            pos.X += dir.X * amount;
            pos.Y += dir.Y * amount;
        }

        public void Turn(float amount)
        {
            Vector2 oldDir = dir;
            dir.X = (float)(dir.X * Math.Cos(amount) - dir.Y * Math.Sin(amount));
            dir.Y = (float)(oldDir.X * Math.Sin(amount) + dir.Y * Math.Cos(amount));

            Vector2 oldPlane = plane;
            plane.X = (float)(plane.X * Math.Cos(amount) - plane.Y * Math.Sin(amount));
            plane.Y = (float)(oldPlane.X * Math.Sin(amount) + plane.Y * Math.Cos(amount));
        }
    }
}