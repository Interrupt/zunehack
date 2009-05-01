using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    class CollisionResult
    {
        protected Vector2 atPos;
        protected float linePosAt;
        protected float lineOrigPosAt;

        public Vector2 CollisionPos { get { return atPos; } }
        public float LinePos { get { return linePosAt; } }
        public float LineOrigPos { get { return lineOrigPosAt; } }

        public CollisionResult(Vector2 pos, float LinePosAt, float FirstLinePosAt)
        {
            atPos = pos;
            linePosAt = LinePosAt;
            lineOrigPosAt = FirstLinePosAt;
        }
    }

    class CollisionHelpers
    {
        /// <summary>
        /// Checks for an intersection between two lines
        /// </summary>
        public static CollisionResult LineLineIntersection(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
        {
            CollisionResult result = null;

            float ua = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
            float ub = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X - point3.X);
            float denominator = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);

            if (Math.Abs(denominator) <= 0.00001f)
            {
                if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                {
                    Vector2 intersectionPoint = (point1 + point2) / 2;
                    result = new CollisionResult(intersectionPoint, ua / denominator, ub / denominator);
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    Vector2 intersectionPoint = new Vector2();
                    intersectionPoint.X = point1.X + ua * (point2.X - point1.X);
                    intersectionPoint.Y = point1.Y + ua * (point2.Y - point1.Y);
                    result = new CollisionResult(intersectionPoint, ub, ua);
                }
            }

            return result;
        }
    }

    class RayHelpers
    {
        public static float AngleBetweenRays(Vector2 dirOne, Vector2 dirTwo)
        {
            return ((dirOne.X - dirTwo.X) > 0 ? 1 : -1) *
                    (float)Math.Acos((double)Vector2.Dot(Vector2.Normalize(dirTwo),
                                                                          Vector2.Normalize(dirOne)));
        }

        public static Vector2 rotate(Vector2 dir, double amount)
        {
            Vector2 oldDir = dir;
            dir.X = (float)(dir.X * Math.Cos(amount) - dir.Y * Math.Sin(amount));
            dir.Y = (float)(oldDir.X * Math.Sin(amount) + dir.Y * Math.Cos(amount));
            return dir;
        }
    }
}