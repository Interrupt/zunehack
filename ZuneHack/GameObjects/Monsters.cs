using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    // Used when creating a monster from a file
    public struct MonsterData
    {
        public string name;
        public string image;
        public int level;
        public ZuneHack.Attributes attribs;
    }

    /// <summary>
    /// A generic monster class that can be loaded from data
    /// </summary>
    public class Monster : NpcActor
    {
        public Monster(MonsterData data, Vector2 startPos)
        {
            name = data.name;

            // Get or load a texture
            texture = GameManager.GetInstance().GetTexture(data.image);
            if (texture == null) texture = GameManager.GetInstance().LoadTexture(data.image);

            pos = startPos;
            displayPos = pos;

            attributes.agility = data.attribs.agility;
            attributes.speed = data.attribs.speed;
            attributes.strength = data.attribs.strength;
            attributes.constitution = data.attribs.constitution;
            attributes.endurance = data.attribs.endurance;

            stats.Initialize(data.level, attributes);
        }
    }
}