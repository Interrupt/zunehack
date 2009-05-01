using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
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
            attributes.constitution = 3;
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

            attributes.agility = 3;
            attributes.speed = 4;
            attributes.strength = 4;
            attributes.constitution = 3;
            attributes.endurance = 3;

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

            attributes.agility = 4;
            attributes.speed = 4;
            attributes.strength = 2;
            attributes.constitution = 2;
            attributes.endurance = 2;

            stats.Initialize(level, attributes);
        }
    }
}