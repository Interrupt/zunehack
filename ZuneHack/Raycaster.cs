using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    class ScreenSplice
    {
        public ScreenSplice(Texture2D Texture, int X, int LineHeight, float Distance, float TexPos, Color Color)
        {
            texture = Texture;
            x = X;
            lineHeight = LineHeight;
            distance = Distance;
            texPos = TexPos;
            color = Color;

            // The z order of this slice
            z = distance / 100.0f;
            if (z > 1) z = 1;
        }

        public Texture2D texture;
        public int x;
        public int lineHeight;
        public float distance;
        public float texPos;
        public Color color;
        public float z;
    }

    class Raycaster
    {
        protected Camera camera;
        protected Map map;
        public SpriteBatch spriteBatch;
        public float posOffset;

        protected int numSplices;
        protected int spliceWidth;
        protected int screenDispHeight;

        List<ScreenSplice>[] splices;

        public Raycaster(Camera cam, SpriteBatch batch, float CameraOffset)
        {
            camera = cam;
            spriteBatch = batch;
            posOffset = CameraOffset;

            numSplices = 100;                   //Number of screen splices for drawing
            spliceWidth = 320 / numSplices;     //Width of each splice, in pixels
            int screenDispHeight = (240 / numSplices) / 2;

            // Due to some rounding depending on the number of splices,
            // some extra splices may need to be drawn
            int excessSplices = (320 - numSplices * spliceWidth) / spliceWidth;
            if (excessSplices < 320) excessSplices++;
            numSplices = numSplices + excessSplices;

            splices = new List<ScreenSplice>[numSplices];
        }

        public void SetCamera(Camera newCam)
        {
            camera = newCam;
        }

        public Camera GetCamera()
        {
            return camera;
        }

        public void SetMap(Map newMap)
        {
            map = newMap;
        }

        public Map GetMap(Map map)
        {
            return map;
        }

        public Vector2 OffsetPos
        {
            get { return camera.pos + (camera.dir * posOffset); }
        }

        /// <summary>
        /// Adds a splice to the list of splices to render
        /// </summary>
        protected void AddSplice(ScreenSplice splice)
        {
            if (splices[splice.x] == null)
            {
                splices[splice.x] = new List<ScreenSplice>();
            }

            splices[splice.x].Add(splice);
        }

        public void Update()
        {
            int width = numSplices;
            int screenWidth = spliceWidth;
            int height = screenDispHeight;

            for (int x = 0; x < width; x++)
            {
                // Clears the list of splices for this column from the last update
                if (splices[x] != null)
                {
                    splices[x].Clear();
                    splices[x] = null;
                }

                float cameraX = 2 * x / (float)width - 1;
                Vector2 rayPos = new Vector2(OffsetPos.X, OffsetPos.Y);
                Vector2 rayDir = new Vector2(camera.dir.X + camera.plane.X * cameraX, camera.dir.Y + camera.plane.Y * cameraX);

                // The current map location
                Vector2 mapPos = new Vector2((int)rayPos.X, (int)rayPos.Y);

                // Length of ray from current position to next column / row sides
                Vector2 sideDist = new Vector2();

                // Length of ray from one side to the next matching side
                Vector2 deltaDist = new Vector2(
                    (float)Math.Sqrt(1 + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X)),
                    (float)Math.Sqrt(1 + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y)));

                float wallDistance;
                int stepX, stepY;       // The direction to step in
                bool hit = false;       // Have we hit a wall?
                bool columnSide = false;        // Was this a column or row side?

                // Calculate step and initial sideDist
                if (rayDir.X < 0)
                {
                    stepX = -1;
                    sideDist.X = (rayPos.X - mapPos.X) * deltaDist.X;
                }
                else
                {
                    stepX = 1;
                    sideDist.X = (mapPos.X + 1.0f - rayPos.X) * deltaDist.X;
                }

                if (rayDir.Y < 0)
                {
                    stepY = -1;
                    sideDist.Y = (rayPos.Y - mapPos.Y) * deltaDist.Y;
                }
                else
                {
                    stepY = 1;
                    sideDist.Y = (mapPos.Y + 1.0f - rayPos.Y) * deltaDist.Y;
                }

                // Check DDA hits
                while (hit == false)
                {
                    // Jump to next map square, in x or y directions
                    if (sideDist.X < sideDist.Y)
                    {
                        sideDist.X += deltaDist.X;
                        mapPos.X += stepX;
                        columnSide = false;
                    }
                    else
                    {
                        sideDist.Y += deltaDist.Y;
                        mapPos.Y += stepY;
                        columnSide = true;
                    }
                    // Checks if the ray has hit a wall
                    if (map.checkHit((int)mapPos.X, (int)mapPos.Y)) hit = true;
                }

                // Calculate distance onto projected camera direction
                if (columnSide == false)
                    wallDistance = Math.Abs(((int)mapPos.X - rayPos.X + (1 - stepX) / 2) / rayDir.X);
                else
                    wallDistance = Math.Abs(((int)mapPos.Y - rayPos.Y + (1 - stepY) / 2) / rayDir.Y);

                int lineHeight = (int)Math.Abs((int)(240 / (wallDistance)));

                // Texture calculations
                float wallX; //Where the wall was hit
                if (columnSide == true)
                    wallX = rayPos.X + (((int)mapPos.Y - rayPos.Y + (1 - stepY) / 2) / rayDir.Y) * rayDir.X;
                else
                    wallX = rayPos.Y + (((int)mapPos.X - rayPos.X + (1 - stepX) / 2) / rayDir.X) * rayDir.Y;

                wallX -= (float)Math.Floor((wallX));

                // X coord on the texture
                int texSize = 128;
                int texX = (int)(wallX * (float)texSize);
                if (columnSide == false && rayDir.X > 0) texX = texSize - texX - 1;
                if (columnSide == true && rayDir.Y < 0) texX = texSize - texX - 1;
                float texPos = texX / (float)texSize;

                int texNum = map.GetTextureNumAt((int)mapPos.X, (int)mapPos.Y);
                CreateSlice(map.mapTextures[texNum], screenWidth, x, lineHeight, wallDistance, columnSide, texPos);

                // Draw any entity flats that need to be raycast
                CreateEntitySlices(rayPos, rayDir, wallDistance, screenWidth, x);
            }
        }

        public void Draw()
        {
            for (int x = 0; x < numSplices; x++)
            {
                if (splices[x] != null)
                {
                    // Draws the splices for this column.
                    for(int i = 0; i < splices[x].Count; i++)
                    {
                        ScreenSplice drawSplice = splices[x][i];
                        spriteBatch.Draw(drawSplice.texture, new Rectangle(spliceWidth * x, -drawSplice.lineHeight / 2 + 240 / 2, spliceWidth, drawSplice.lineHeight),
                        new Rectangle((1 * (int)(drawSplice.texture.Width * drawSplice.texPos)), 0, 1, drawSplice.texture.Height), drawSplice.color, 0, new Vector2(), SpriteEffects.None, drawSplice.z);
                    }
                }
            }

            // Draw the entities, or quit if none
            if (map.entities == null) return;
            for (int x = 0; x < map.entities.Count; x++)
            {
                Entity entity = map.entities[x];
                if(entity.directional == false)
                    DrawEntity(entity);
            }
        }

        protected void DrawEntity(Entity entity)
        {
            float spriteX = entity.displayPos.X - OffsetPos.X;
            float spriteY = entity.displayPos.Y - OffsetPos.Y;

            float invDet = 1.0f / camera.GetFOV();
            float transformX = invDet * (camera.dir.Y * spriteX - camera.dir.X * spriteY);
            float transformY = invDet * (-camera.plane.Y * spriteX + camera.plane.X * spriteY);

            if (transformY > 0)
            {
                int spriteScreenX = (int)((320 / 2) * (1 + transformX / transformY));

                int spriteHeight = Math.Abs((int)(240 / transformY));
                int drawStartY = -spriteHeight / 2 + 240 / 2;
                int drawStartX = -spriteHeight / 2 + spriteScreenX;

                // Skips drawing the entity if it's off the left or right of the screen
                if (drawStartX > 320 || drawStartX + spriteHeight < 0) return;

                // Compute the z-order of the sprite
                float z = transformY / 100.0f;
                if (z > 1) z = 1;

                // Shade the sprite depending on it's distance.
                float distanceScale = (spriteHeight / map.distanceShadingScale);
                distanceScale = distanceScale > 1.0 ? 1.0f : distanceScale;

                Color multColor = map.rowSideShading;
                multColor.R = (byte)(multColor.R * distanceScale);
                multColor.G = (byte)(multColor.G * distanceScale);
                multColor.B = (byte)(multColor.B * distanceScale);

                spriteBatch.Draw(entity.texture, new Rectangle(drawStartX, drawStartY, spriteHeight, spriteHeight), null, multColor, 0, new Vector2(0, 0), SpriteEffects.None, z);
            }
        }

        protected void CreateSlice(Texture2D brushtex, int screenWidth, int x, int lineHeight, float wallDistance, bool columnSide, float texPos)
        {
            // Set the starting color based on the side displayed
            Color multColor = map.rowSideShading;
            if (columnSide == true) multColor = map.columnSideShading;

            // Shade the column depending on it's distance.
            float distanceScale = (lineHeight / map.distanceShadingScale);
            distanceScale = distanceScale > 1.0 ? 1.0f : distanceScale;

            multColor.R = (byte)(multColor.R * distanceScale);
            multColor.G = (byte)(multColor.G * distanceScale);
            multColor.B = (byte)(multColor.B * distanceScale);

            ScreenSplice newSplice = new ScreenSplice(brushtex, x, lineHeight, wallDistance, texPos, multColor);
            AddSplice(newSplice);
        }

        protected void CreateEntitySlices(Vector2 rayPos, Vector2 rayDir, float hitDistance, int screenWidth, int x)
        {
            if (map.entities == null) return;

            for(int i = 0; i < map.entities.Count; i++)
            {
                Entity entity = map.entities[i];
                if (entity.directional)
                {
                    CollisionResult col = CollisionHelpers.LineLineIntersection(rayPos, rayPos + (rayDir * (hitDistance + 0.2f)), entity.pos, entity.pos + entity.dir);
                    if (col != null)
                    {
                        Vector2 dist = col.CollisionPos - rayPos;

                        // Quick way to find the angle between the current ray dir and the camera
                        /*float startAngle = camera.GetFOV();
                        float endAngle = -0.574730933f;
                        float rayAngle = ((float)x / (numSplices - 1)) * (endAngle - startAngle) + startAngle;*/

                        float rayAngle = RayHelpers.AngleBetweenRays(rayDir, camera.dir);

                        float distance = (float)(Math.Abs(dist.Length()) * Math.Cos(rayAngle));
                        int lineHeight = (int)Math.Abs((int)(240 / (distance)));

                        CreateSlice(entity.texture, screenWidth, x, lineHeight, distance, true, col.LinePos);
                    }
                }
            }
        }
    }
}