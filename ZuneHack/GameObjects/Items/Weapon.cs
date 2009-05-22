using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuneHack
{
    public enum WeaponClass
    {
        Blade = 1,
        Axe,
        Blunt,
        Piercing
    }

    // Used when creating a monster from a file
    public struct WeaponData
    {
        public string name;
        public string image;
        public int level;
        public WeaponClass wpnclass;
        public int damage;
        public int bonus;
    }

    public class Weapon : Item
    {
        protected int damage;
        protected int bonus;
        protected WeaponClass weaponClass;

        public int Damage { get { return damage; } }
        public int Bonus { get { return bonus; } }
        public WeaponClass WeaponClass { get { return weaponClass; } }

        /// <summary>
        /// Creates a weapon
        /// </summary>
        public Weapon(WeaponData data) : base(ItemType.Weapon)
        {
            name = data.name;

            // Get or load a texture
            texture = GameManager.GetInstance().GetTexture(data.image);
            if (texture == null) texture = GameManager.GetInstance().LoadTexture(data.image);

            damage = data.damage;
            bonus = data.bonus;
            weaponClass = data.wpnclass;
        }
    }
}
