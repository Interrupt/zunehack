using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuneHack
{
    public enum ArmorBodySlot
    {
        Head = 1,
        Feet,
        Body,
        Arms,
        Offhand
    }

    // Used when creating armor items from a file
    public struct ArmorData
    {
        public string name;
        public string image;
        public int level;
        public ArmorBodySlot slot;
        public int armor;
        public int bonus;
        public int chance;
    }

    public class Armor : Item
    {
        protected int armor;
        protected int bonus;
        protected ArmorBodySlot slot;

        public int Protection { get { return armor; } }
        public int Bonus { get { return bonus; } }
        public ArmorBodySlot Slot { get { return slot; } }

        /// <summary>
        /// Creates an armor item
        /// </summary>
        public Armor(ArmorData data)
            : base(ItemType.Armor)
        {
            name = data.name;

            // Get or load a texture
            texture = GameManager.GetInstance().GetTexture(data.image);
            if (texture == null) texture = GameManager.GetInstance().LoadTexture(data.image);

            armor = data.armor;
            bonus = data.bonus;
            slot = data.slot;
        }
    }
}
