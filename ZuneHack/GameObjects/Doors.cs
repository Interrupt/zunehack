using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    class Door : Entity
    {
        public float movePercentage;
        public float moveSpeed;
        protected bool isOpen;
        protected double endRadians;
        protected Vector2 closedDir;

        public Door(Vector2 position, string tex, bool isClosed, bool Directional)
        {
            pos = position;
            displayPos = pos;
            texture = GameManager.GetInstance().GetTexture(tex);
            blockMovement = isClosed;
            isOpen = !isClosed;
            directional = Directional;
            movePercentage = 1;
            closedDir = dir;
        }

        public override void Update(float timescale)
        {
            blockMovement = !isOpen;

            if (movePercentage < 1)
            {
                movePercentage += moveSpeed * timescale;
                if (movePercentage > 1) movePercentage = 1;

                double newRadians = movePercentage * endRadians;
                dir = RayHelpers.rotate(closedDir, newRadians);
            }
        }

        public void Toggle(float speed)
        {
            moveSpeed = speed;
            movePercentage = 0;

            blockMovement = !isOpen;
            isOpen = !isOpen;

            endRadians = (2.0 * Math.PI) / 4.2; // A rotation of 1/4th of a circle
        }
    }
}