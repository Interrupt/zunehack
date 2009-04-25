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
        }

        public override void Initialize()
        {
            // Sets the initial direction of the door based on adjacent open squares
            if (!ownerMap.checkHit(pos + dir))
            {
                dir = new Vector2(-1, 0);
            }

            // Sets the closed direction and position. Position needs to be set to make the door flush with the wall.
            closedDir = dir;
            pos -= (closedDir * 0.49f);
            pos += (RayHelpers.rotate(closedDir,  (-2.0 * Math.PI) / 4.0)) * 0.45f;
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