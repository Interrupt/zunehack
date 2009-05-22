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
