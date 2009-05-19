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
        Potion,
        Armor,
        Food,
        Scroll,
        Wand
    }

    public enum PotionType
    {
        Health = 1,
        Magic,
        Speed,
        Poison
    }

    public enum WeaponClass
    {
        Blade = 1,
        Axe,
        Blunt,
        Piercing
    }

    public enum WeaponType
    {
        Dagger = 1,
        Shortsword ,
        Longsword,
        Axe,
        Waraxe,
        Club,
        Staff,
        Spear
    }

    public enum WeaponQuality
    {
        Iron = 1,
        Steel,
        Silver,
        Dragonbone,
        Vorpal
    }

    public enum ArmourClass
    {
        Leather = 1,
        Chainmail,
        Scalemail,
        Platemail
    }

    public enum ArmourType
    {
        Helmet = 1,
        Boots,
        Chest,
        Grieves
    }

    public class Item : Entity
    {
        protected string name;
        protected ItemType type;
        protected int amount;
        protected bool stackable;

        public Item(ItemType theType)
        {
            type = theType;
        }

        public Item(ItemType theType, string theName)
        {
            type = theType;
            name = theName;
        }

        public Item(ItemType theType, string theName, bool Stackable)
        {
            type = theType;
            name = theName;
            stackable = Stackable;
        }

        public void SetStackable(bool val)
        {
            stackable = val;
        }

        public string Name { get { return name; } set { name = value; } }
        public ItemType Type { get { return type; } }
        public int Amount { get { return amount; } set { amount = value; } }
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

    public class Weapon : Item
    {
        protected int damage;
        protected int bonus;
        protected WeaponClass weaponClass;
        protected WeaponType weaponType;
        protected WeaponQuality weaponQuality;

        public int Damage { get { return damage; } }
        public int Bonus { get { return bonus; } }
        public WeaponClass WeaponClass { get { return weaponClass; } }
        public WeaponType WeaponType { get { return weaponType; } }
        public WeaponQuality WeaponQuality { get { return weaponQuality; } }

        /// <summary>
        /// Sets some weapon attributes based on the type
        /// </summary>
        public Weapon(WeaponType theType) : base(ItemType.Weapon)
        {
            switch (theType)
            {
                case WeaponType.Dagger:
                    name = "dagger";
                    weaponClass = WeaponClass.Blade;
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    damage = 2;
                    break;
                case WeaponType.Longsword:
                    name = "longsword";
                    weaponClass = WeaponClass.Blade;
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    damage = 6;
                    break;
                case WeaponType.Shortsword:
                    name = "shortsword";
                    weaponClass = WeaponClass.Blade;
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    damage = 4;
                    break;
                case WeaponType.Axe:
                    name = "axe";
                    weaponClass = WeaponClass.Axe;
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    damage = 5;
                    break;
                case WeaponType.Waraxe:
                    name = "waraxe";
                    weaponClass = WeaponClass.Axe;
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    damage = 7;
                    break;
                case WeaponType.Club:
                    name = "club";
                    weaponClass = WeaponClass.Blunt;
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    damage = 3;
                    break;
                case WeaponType.Staff:
                    name = "staff";
                    weaponClass = WeaponClass.Blunt;
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    damage = 6;
                    break;
                case WeaponType.Spear:
                    name = "spear";
                    weaponClass = WeaponClass.Piercing;
                    texture = GameManager.GetInstance().GetTexture(@"Items\sword");
                    damage = 8;
                    break;
            }
        }
    }

    public class ItemCreator
    {
        public static Item CreateGold(int goldAmount)
        {
            Item newItem = new Item(ItemType.Money, String.Format("{0} gold pieces", goldAmount));
            newItem.Amount = goldAmount;
            newItem.texture = GameManager.GetInstance().GetTexture(@"Items\moneybag");
            newItem.SetStackable(true);
            return newItem;
        }
    }
}
