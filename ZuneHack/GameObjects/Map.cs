using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZuneHack.Generation;

namespace ZuneHack
{
    public enum MapType
    {
        dungeon,
        forest,
        cave
    }

    public class Map
    {
        public int[,] mapData;
        protected int width;
        protected int height;
        public List<Entity> entities;

        public Texture2D[] mapTextures;
        public Color columnSideShading = Color.LightGray;
        public Color rowSideShading = Color.White;
        public float distanceShadingScale = 180.0f;

        public Color ceilingColor = new Color(0.15f,0.15f,0.15f);
        public Color floorColor = new Color(0.25f, 0.25f, 0.25f);

        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }

        public int level;

        /// <summary>
        /// Generates a default test map
        /// </summary>
        public Map()
        {
            width = 24;
            height = 24;

            mapData = new int[24, 24] {
            {1,1,2,1,3,1,2,1,2,1,1,1,3,1,2,1,1,2,1,3,1,1,1,1},
            {1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,2,0,1,0,2,0,2,0,0,0,0,0,1,0,0,0,0,0,0,1},
            {1,0,0,1,1,0,0,0,0,0,2,1,1,1,0,2,1,3,0,0,0,0,0,1},
            {1,0,2,0,3,1,2,0,1,0,2,0,0,1,0,0,0,1,0,0,3,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,1,0,2,0,0,0,0,0,1,0,0,0,0,0,0,1,4,1,1,0,0,1},
            {1,0,1,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,1,0,1,1,3,1,0,1,4,1,0,0,0,1,2,2,0,0,0,0,0,1},
            {2,0,2,0,0,0,0,1,0,4,0,4,1,2,0,4,0,2,1,0,1,1,2,1},
            {2,0,2,0,0,0,0,1,0,1,4,1,0,0,0,1,1,1,0,0,0,0,0,1},
            {1,0,1,1,1,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,2,0,1,0,0,0,1,1,0,2,4,1,0,2,0,2,0,1,1},
            {1,0,2,0,0,2,0,4,0,0,0,1,0,0,0,2,2,2,2,0,2,0,0,1},
            {1,0,0,0,0,2,0,1,0,2,0,2,0,0,0,2,0,0,0,0,2,0,1,1},
            {1,1,1,0,1,1,0,1,1,0,1,0,0,0,0,2,0,2,2,2,2,0,0,1},
            {1,0,0,0,0,0,0,0,0,2,0,2,1,0,1,2,0,2,0,0,0,0,1,1},
            {1,0,0,3,0,0,0,0,0,1,0,0,0,0,0,1,0,2,2,2,2,0,0,1},
            {1,0,0,4,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,4,1,0,2,1,1,2,3,0,1,2,2,1,1,0,1,0,0,0,1,0,1},
            {1,0,0,1,0,2,0,0,0,0,0,0,0,0,0,1,0,1,0,1,1,1,0,1},
            {1,0,0,1,0,0,0,0,4,0,0,0,0,0,0,1,0,1,1,1,1,1,0,1},
            {1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };

            // Sets the textures to use as map tiles
            int mdx = 0;
            mapTextures = new Texture2D[6];
            mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\brick-grey");
            mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\brick-mossy");
            mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\brick-bloody");
            mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\brick-torch");
            mapTextures[mdx++] = GameManager.GetInstance().GetTexture(@"Walls\door");

            AddEntity(new Rat(1, new Vector2(20.5f, 12.5f)));
            AddEntity(new Kobold(1, new Vector2(1.5f, 19.5f)));
            AddEntity(new Goblin(1, new Vector2(2.5f, 2.5f)));
            AddEntity(new Entity(new Vector2(2.5f, 3.5f), @"Deco\column", true));
            AddEntity(new Entity(new Vector2(3.5f, 2.5f), @"Deco\column", true));
            AddEntity(new Door(new Vector2(19.5f, 10.5f), @"Walls\door", true, true));
            AddEntity(new Door(new Vector2(20.5f, 8.5f), @"Walls\door-grate", true, true));
            AddEntity(new Door(new Vector2(4.5f, 1.5f), @"Walls\door", true, true));
        }

        /// <summary>
        /// Creates a map via an array of map data and a list of entities
        /// </summary>
        public Map(int[,] data, int Width, int Height, List<Entity> Entities)
        {
            mapData = data;
            width = Width;
            height = Height;
            entities = Entities;
        }

        /// <summary>
        /// Generates a map based on the dungeon level and type
        /// </summary>
        public Map(int Level, MapType type)
        {
            MapGenerator generator = new MapGenerator(type, this);
            level = Level;
        }

        /// <summary>
        ///  Sets the level data
        /// </summary>
        public void setData(int Width, int Height, int[,] data)
        {
            mapData = data;
            width = Width;
            height = Height;
        }

        /// <summary>
        /// Adds an entity to the map's entity list
        /// </summary>
        public void AddEntity(Entity newEntity)
        {
            if (entities == null) entities = new List<Entity>();

            entities.Add(newEntity);
            newEntity.SetMap(this);
            newEntity.Initialize();
        }

        public void SetShading(Color colSide, Color rowSide)
        {
            columnSideShading = colSide;
            rowSideShading = rowSide;
        }

        public void SetTextures(Texture2D[] Textures)
        {
            mapTextures = Textures;
        }

        public bool checkHit(int x, int y)
        {
            return mapData[x, y] > 0;
        }

        public bool checkHit(Vector2 checkLoc)
        {
            if (checkLoc.X > 0 && checkLoc.X < width - 1 && checkLoc.Y > 0 && checkLoc.Y < height - 1)
                return checkHit((int)checkLoc.X, (int)checkLoc.Y);
            return true;
        }

        public Entity checkEntityHit(Vector2 checkLoc)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity.blockMovement)
                {
                    bool entityHit = (int)entity.pos.X == (int)checkLoc.X && (int)entity.pos.Y == (int)checkLoc.Y;
                    if (entityHit) return entity;
                }
            }
            return null;
        }

        public Item GetItemAt(Vector2 checkLoc)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Item item = entities[i] as Item;
                if (item != null)
                {
                    bool hit = (int)item.pos.X == (int)checkLoc.X && (int)item.pos.Y == (int)checkLoc.Y;
                    if (hit) return item;
                }
            }
            return null;
        }

        public bool checkMovability(Vector2 checkLoc)
        {
            return checkHit(checkLoc) || checkEntityHit(checkLoc) != null;
        }

        public int GetTextureNumAt(int x, int y)
        {
            if(checkHit(x,y)) return mapData[x, y] - 1;
            return 0;
        }

        public int GetTileAt(int x, int y)
        {
            return mapData[x, y];
        }

        public Vector2 GetStairUpLoc()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mapData[x, y] == -1) return new Vector2(x + 0.5f, y + 0.5f);
                }
            }
            return new Vector2(1.5f, 1.5f);
        }
    }
}