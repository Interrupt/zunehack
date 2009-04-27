using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    class MapGenerator
    {
        MapType type;
        Map map;
        int width;
        int height;

        public MapGenerator(MapType Type, Map Map)
        {
            type = Type;
            map = Map;

            width = 30;
            height = 30;

            map.mapData = new int[width, height];
            map.entities = new List<Entity>();

            SetTypeTextures();

            // Fills map
            FillRect(0, 0, width - 1, height - 1, 1);

            // Generate some rooms
            int numGenerated = 0;
            while (numGenerated < 9)
            {
                bool didGenerate = MakeRoom();
                if (didGenerate) numGenerated++;
            }

            PlaceStairsUp();
            PlaceStairsDown();
        }

        /// <summary>
        /// Fills a square of tiles on the map
        /// </summary>
        protected void FillRect(int startx, int starty, int endx, int endy, int tile)
        {
            for (int x = startx; x < endx; x++)
            {
                for (int y = starty; y < endy; y++)
                {
                    map.mapData[x, y] = tile;
                }
            }
        }

        /// <summary>
        /// Draws a square outline of tiles on the map
        /// </summary>
        protected void OutlineRect(int startx, int starty, int endx, int endy, int tile)
        {
            for (int x = startx; x < endx; x++)
            {
                map.mapData[x, starty] = tile;
                map.mapData[x, endy] = tile;
            }

            for (int y = starty; y < endy; y++)
            {
                map.mapData[startx, y] = tile;
                map.mapData[endx, y] = tile;
            }
        }

        /// <summary>
        /// Makes a random room, return false if it couldn't generate
        /// </summary>
        protected bool MakeRoom()
        {
            Random rnd = new Random();

            int roomWidth = 3 + rnd.Next(8);
            int roomHeight = 3 + rnd.Next(8);
            
            int x = rnd.Next(1, width - roomWidth - 2);
            int y = rnd.Next(1, height - roomHeight - 2);

            if (CheckArea(x, y, x + roomWidth, y + roomWidth, 0)) return false;

            // Carve out this rectangle of the map
            FillRect(x, y, x + roomWidth, y + roomWidth, 0);

            return true;
        }

        // Checks to see if this area contains the specified tile
        protected bool CheckArea(int startx, int starty, int endx, int endy, int tile)
        {
            for (int x = startx; x < endx; x++)
            {
                for (int y = starty; y < endy; y++)
                {
                    if (map.mapData[x, y] == tile) return true;
                }
            }

            return false;
        }

        // Checks if this area is clear
        protected bool CheckIsClear(int x, int y)
        {
            return map.mapData[x, y] == 0; 
        }

        /// <summary>
        /// Generates the monsters for the map
        /// </summary>
        protected void GenerateMonsters()
        {
            
        }

        protected void PlaceStairsUp()
        {
            Random rnd = new Random();
            bool didPlace = false;

            while (didPlace == false)
            {
                int locX = rnd.Next(1, width - 1);
                int locY = rnd.Next(1, height - 1);
                didPlace = CheckIsClear(locX, locY);

                if (didPlace) map.mapData[locX, locY] = -1;
            }
        }

        protected void PlaceStairsDown()
        {
            Random rnd = new Random();
            bool didPlace = false;

            while (didPlace == false)
            {
                int locX = rnd.Next(1, width - 1);
                int locY = rnd.Next(1, height - 1);
                didPlace = CheckIsClear(locX, locY);

                if (didPlace) map.mapData[locX, locY] = -2;
            }
        }

        protected void SetTypeTextures()
        {
            if (type == MapType.dungeon)
            {
                // Sets the textures to use as map tiles
                int mdx = 0;
                map.mapTextures = new Texture2D[6];
                map.mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\brick-grey");
                map.mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\brick-mossy");
                map.mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\brick-bloody");
                map.mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\brick-torch");
                map.mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\door");
            }
        }
    }
}