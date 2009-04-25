using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    class Map
    {
        protected int[,] mapData;
        protected int width;
        protected int height;
        public List<Entity> entities;

        public Texture2D[] mapTextures;
        public Color columnSideShading = Color.LightGray;
        public Color rowSideShading = Color.White;
        public float distanceShadingScale = 100.0f;

        public Map()
        {
            width = 24;
            height = 24;

            mapData = new int[24, 24] {
            {1,1,2,1,3,1,2,1,2,1,1,1,3,1,2,1,1,2,1,3,1,1,1,1},
            {1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,2,0,1,0,2,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,1,1,0,0,0,0,0,2,0,0,1,0,0,0,0,0,0,0,0,0,1},
            {1,0,2,0,3,1,2,0,1,0,2,0,0,1,0,0,0,1,0,0,3,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,1,0,2,0,0,0,0,0,1,0,0,0,0,0,0,1,4,1,1,0,0,1},
            {1,0,1,0,2,0,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,1,0,1,1,3,1,0,1,4,1,0,0,0,1,2,2,0,0,0,0,0,1},
            {2,0,2,0,0,0,0,1,0,4,0,4,0,0,0,4,0,2,1,0,1,1,2,1},
            {2,0,2,0,0,0,0,1,0,1,4,1,0,0,0,1,1,1,0,0,0,0,0,1},
            {1,0,1,1,1,1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,2,0,1,0,0,0,1,0,1,2,4,1,0,2,0,2,0,1,1},
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

            AddEntity(new Actor(new Vector2(20.5f, 12.5f), "rat"));
            AddEntity(new Actor(new Vector2(1.5f, 19.5f), "kobold"));
            AddEntity(new Actor(new Vector2(2.5f, 2.5f), "goblin"));
            AddEntity(new Entity(new Vector2(2.5f, 3.5f), @"Deco\column", true));
            AddEntity(new Entity(new Vector2(3.5f, 2.5f), @"Deco\column", true));
            AddEntity(new Door(new Vector2(19.5f, 10.5f), @"Walls\door", true, true));
            AddEntity(new Door(new Vector2(20.5f, 8.5f), @"Walls\door-grate", true, true));
            AddEntity(new Door(new Vector2(4.5f, 1.5f), @"Walls\door", true, true));
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

        public Map(int[,] data, int Width, int Height, List<Entity> Entities)
        {
            mapData = data;
            width = Width;
            height = Height;
            entities = Entities;
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

        public bool checkMovability(Vector2 checkLoc)
        {
            return checkHit(checkLoc) || checkEntityHit(checkLoc) != null;
        }

        public int GetTextureNumAt(int x, int y)
        {
            if(checkHit(x,y)) return mapData[x, y] - 1;
            return 0;
        }
    }
}