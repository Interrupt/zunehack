using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack.Generation
{
    enum Direction
    {
        north = 1,
        east,
        south,
        west
    }

    /// <summary>
    /// Data for a generated room feature
    /// </summary>
    struct Room
    {
        public int width;
        public int height;
        public int posX;
        public int posY;
    }

    /// <summary>
    /// Connections between features
    /// </summary>
    struct Connector
    {
        public bool noDoor;
        public int posX;
        public int posY;
        public Direction dir;
    }

    class MapGenerator
    {
        MapType type;
        Map map;
        int width;
        int height;
        List<Connector> available_conns;
        Random rnd;

        public MapGenerator(MapType Type, Map Map)
        {
            // List of map connections available
            available_conns = new List<Connector>();
            rnd = new Random();

            type = Type;
            map = Map;

            width = 50;
            height = 50;

            map.mapData = new int[width, height];
            map.Width = width;
            map.Height = height;

            map.entities = new List<Entity>();

            SetTypeTextures();

            // Fills map
            FillRandRect(0, 0, width - 1, height - 1, 1, 3);

            // Randomly place the first connector
            Connector connection = new Connector();
            connection.posX = width / 2;
            connection.posY = height / 2;
            connection.dir = GetRandomDirection();
            connection.noDoor = true;

            available_conns.Add(connection);

            // Generate some rooms
            int numGenerated = 0;
            int tries = 0;
            while (numGenerated < 20 && tries < 30 && available_conns.Count > 0)
            {
                // Attempt to make a room with the first connector
                Connector tryThis = available_conns.First();
                bool didGenerate = MakeRoom(tryThis);
                if (didGenerate)
                {
                    // This connection generated fine, remove the connector
                    numGenerated++;
                    available_conns.Remove(tryThis);
                }
                else
                {
                    // Check to see if we've tried too many times already
                    if (tries++ > 2)
                    {
                        tries = 0;
                        available_conns.Remove(tryThis);
                    }
                }
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
        /// Fills a square of tiles on the map with random data
        /// </summary>
        protected void FillRandRect(int startx, int starty, int endx, int endy, int startRand, int endRand)
        {
            for (int x = startx; x < endx; x++)
            {
                for (int y = starty; y < endy; y++)
                {
                    map.mapData[x, y] = rnd.Next(startRand, endRand);
                }
            }
        }

        protected void SetTile(int x, int y, int tile)
        {
            map.mapData[x, y] = tile;
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
        protected bool MakeRoom(Connector connection)
        {
            int roomWidth = 2 + rnd.Next(4);
            int roomHeight = 2 + rnd.Next(4);
            
            int x = rnd.Next(1, roomWidth - 1);
            int y = rnd.Next(1, roomHeight - 1);

            if (connection.dir == Direction.east)
                x = -1;
            else if (connection.dir == Direction.north)
                y = roomHeight;
            else if (connection.dir == Direction.south)
                y = -1;
            else if (connection.dir == Direction.west)
                x = roomWidth;

            int placeX = connection.posX - x;
            int placeY = connection.posY - y;

            Rectangle roomRect = new Rectangle(placeX, placeY, roomWidth, roomHeight);
            if (!AreaInBounds(roomRect) || CheckArea(roomRect, 0)) return false;

            // Carve out this rectangle of the map
            FillRect(placeX, placeY, placeX + roomWidth, placeY + roomHeight, 0);

            // Place the door
            if (!connection.noDoor)
            {
                SetTile(connection.posX, connection.posY, -3);
                Door newDoor = new Door(new Vector2(connection.posX + 0.5f, connection.posY + 0.5f), @"Walls\door", true, true);
                map.AddEntity(newDoor);
            }

            // Add some new connections
            int numCon = rnd.Next(2, 5);
            for (int i = 0; i < numCon; i++)
            {
                Connector newConnector = new Connector();
                newConnector.dir = GetRandomDirection();
                newConnector.posX = rnd.Next(placeX, placeX + roomWidth);
                newConnector.posY = rnd.Next(placeY, placeY + roomHeight);

                if (newConnector.dir == Direction.north)
                    newConnector.posY = placeY - 1;
                else if (newConnector.dir == Direction.south)
                    newConnector.posY = placeY + roomHeight;
                else if (newConnector.dir == Direction.east)
                    newConnector.posX = placeX + roomWidth;
                else if (newConnector.dir == Direction.west)
                    newConnector.posX = placeX - 1;

                available_conns.Add(newConnector);

                // Add some monsters, perhaps
                int genNum = rnd.Next(-2, 2);
                for (int m = 0; m < genNum; m++)
                {
                    AddRandomMonster(roomRect);
                }
            }

            return true;
        }

        // Checks to see if this area contains the specified tile
        protected bool CheckArea(Rectangle rect, int tile)
        {
            rect.Inflate(1, 1);
            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    if (map.mapData[x, y] == tile) return true;
                }
            }
            return false;
        }

        // Checks to see if this area is in bounds
        protected bool AreaInBounds(Rectangle rect)
        {
            rect.Inflate(1, 1);
            if (rect.Left < 1 || rect.Left > width - 2) return false;
            if (rect.Right < 1 || rect.Right > width - 2) return false;
            if (rect.Top < 1 || rect.Top > width - 2) return false;
            if (rect.Bottom < 1 || rect.Bottom > width - 2) return false;
            return true;
        }

        // Checks if this area is clear
        protected bool CheckIsClear(int x, int y)
        {
            return map.mapData[x, y] == 0; 
        }

        /// <summary>
        /// Generates the monsters for the map
        /// </summary>
        protected void AddRandomMonster(Rectangle area)
        {
            if (type == MapType.dungeon)
            {
                int mt = rnd.Next(1, 4);
                Actor m = null;

                if (mt == 1)
                    m = new Kobold(1, new Vector2(rnd.Next(area.Left, area.Right) + 0.5f, rnd.Next(area.Top, area.Bottom) + 0.5f));
                else if (mt == 2)
                    m = new Rat(1, new Vector2(rnd.Next(area.Left, area.Right) + 0.5f, rnd.Next(area.Top, area.Bottom) + 0.5f));
                else if (mt == 3)
                    m = new Goblin(1, new Vector2(rnd.Next(area.Left, area.Right) + 0.5f, rnd.Next(area.Top, area.Bottom) + 0.5f));

                if (m != null) map.AddEntity(m);
            }
        }

        protected void PlaceStairsUp()
        {
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

        /// <summary>
        /// Returns a random direction
        /// </summary>
        protected Direction GetRandomDirection()
        {
            return (Direction)rnd.Next(1, 5);
        }

        /// <summary>
        /// Checks whether two directions are opposite
        /// </summary>
        protected static bool AreOpposite(Direction first, Direction second)
        {
            // If the numbers are both even or both odd, then they're opposite
            int f = (int)first % 2;
            int s = (int)second % 2;
            return f == s;
        }
    }
}