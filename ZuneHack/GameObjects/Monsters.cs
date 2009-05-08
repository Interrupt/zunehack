using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
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

    public class Rat : NpcActor
    {
        public Rat(int level, Vector2 startPos)
        {
            name = "rat";
            texture = GameManager.GetInstance().GetTexture(@"Actors\rat");
            pos = startPos;
            displayPos = pos;

            attributes.agility = 6;
            attributes.speed = 5;
            attributes.strength = 1;
            attributes.constitution = 2;
            attributes.endurance = 3;

            stats.Initialize(level, attributes);
        }
    }

    public class Goblin : NpcActor
    {
        public Goblin(int level, Vector2 startPos)
        {
            name = "goblin";
            texture = GameManager.GetInstance().GetTexture(@"Actors\goblin");
            pos = startPos;
            displayPos = pos;

            attributes.agility = 4;
            attributes.speed = 4;
            attributes.strength = 3;
            attributes.constitution = 4;
            attributes.endurance = 4;

            stats.Initialize(level, attributes);

        }
    }

    public class Kobold : NpcActor
    {
        public Kobold(int level, Vector2 startPos)
        {
            name = "kobold";
            texture = GameManager.GetInstance().GetTexture(@"Actors\kobold");
            pos = startPos;
            displayPos = pos;

            attributes.agility = 6;
            attributes.speed = 4;
            attributes.strength = 2;
            attributes.constitution = 3;
            attributes.endurance = 2;

            stats.Initialize(level, attributes);
        }
    }
}