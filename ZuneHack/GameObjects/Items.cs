using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuneHack
{
    public enum ItemType
    {
        Money = 1,
        Weapon,
        Food,
        Potion,
        Scroll
    }

    public enum WeaponClass
    {
        Blade = 1,
        Axe,
        Blunt
    }

    public enum WeaponType
    {
        Shortsword = 1,
        Longsword,
        Axe,
        Club,
        Staff
        
    }

    public enum ArmourClass
    {
        Light = 1,
        Heavy
    }

    public class Item : Entity
    {
        protected string name;
        protected ItemType type;
        protected int amount;

        public Item(ItemType theType)
        {
            type = theType;
        }

        public Item(ItemType theType, string theName)
        {
            type = theType;
            name = theName;
        }

        public string Name { get { return name; } set { name = value; } }
        public ItemType Type { get { return type; } }
        public int Amount { get { return amount; } set { amount = value; } }
    }

    public class Weapon : Item
    {
        protected int damage;
        protected int bonus;
        protected WeaponClass weaponClass;
        protected WeaponType weaponType;

        public int Damage { get { return damage; } }
        public int Bonus { get { return bonus; } }
        public WeaponClass Class { get { return weaponClass; } }
        public WeaponType WeaponType { get { return weaponType; } }

        /// <summary>
        /// Sets some weapon attributes based on the type
        /// </summary>
        public Weapon(WeaponType theType) : base(ItemType.Weapon)
        {
            switch (theType)
            {
                case WeaponType.Longsword:
                    weaponClass = WeaponClass.Blade;
                    name = "Longsword";
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    break;
                case WeaponType.Shortsword:
                    weaponClass = WeaponClass.Blade;
                    name = "Shortsword";
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    break;
                case WeaponType.Axe:
                    weaponClass = WeaponClass.Axe;
                    name = "Axe";
                    break;
                case WeaponType.Club:
                    weaponClass = WeaponClass.Blunt;
                    name = "Club";
                    break;
                case WeaponType.Staff:
                    weaponClass = WeaponClass.Blunt;
                    name = "Staff";
                    break;

            }
        }
    }

    public class ItemCreator
    {
        public static Item CreateGold(int goldAmount)
        {
            Item newItem = new Item(ItemType.Money, String.Format("bag of {0} gold", goldAmount));
            newItem.Amount = goldAmount;
            newItem.texture = GameManager.GetInstance().GetTexture(@"Items\moneybag");
            return newItem;
        }

        public static Item CreateHealthPotion()
        {
            Item newItem = new Item(ItemType.Potion, "potion of health");
            newItem.Amount = 10;
            newItem.texture = GameManager.GetInstance().GetTexture(@"Items\potion-red");
            return newItem;
        }
    }
}
