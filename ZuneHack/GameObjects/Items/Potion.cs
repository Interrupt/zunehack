using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuneHack
{
    public enum PotionType
    {
        Health = 1,
        Magic,
        Speed,
        Poison
    }

    public class Potion : Item
    {
        public PotionType potionType;

        public Potion(PotionType theType) : base(ItemType.Potion)
        {
            potionType = theType;
            stackable = true;
            name = "";

            switch (theType)
            {
                case PotionType.Health:
                    texture = GameManager.GetInstance().GetTexture(@"Items\potion-red");
                    name = "dark red";
                    break;
                case PotionType.Magic:
                    texture = GameManager.GetInstance().GetTexture(@"Items\potion-blue");
                    name = "swirly blue";
                    break;
                case PotionType.Poison:
                    texture = GameManager.GetInstance().GetTexture(@"Items\potion-green");
                    name = "bubbly green";
                    break;
                case PotionType.Speed:
                    texture = GameManager.GetInstance().GetTexture(@"Items\potion-orange");
                    name = "fizzy orange";
                    break;
            }

            name += " potion";
        }
    }
}
