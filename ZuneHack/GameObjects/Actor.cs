using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZuneHack
{
    public struct Attributes
    {
        public int strength;
        public int constitution;
        public int endurance;
        public int agility;
        public int speed;
        public int intelligence;

        public bool CheckHit(Attributes theirAttribs)
        {
            Random rnd = GameManager.GetInstance().Random;

            int myCheck = rnd.Next(1, agility);
            int theirCheck = rnd.Next(1, theirAttribs.agility);
            return myCheck >= theirCheck;
        }

        public int CheckMeleeDamage()
        {
            return GameManager.GetInstance().Random.Next(1, strength);
        }
    }

    public struct Stats
    {
        public int level;
        public int maxHealth;
        public int curHealth;
        public int maxMana;
        public int curMana;

        public void Initialize(int Level, Attributes attributes)
        {
            level = Level;
            maxHealth = level * attributes.constitution;
            maxMana = level * attributes.intelligence;
            curHealth = maxHealth;
            curMana = maxMana;
        }
    }

    public class WornItems
    {
        protected Weapon wpn_equipped;
        protected Item held_item;
    }

    /// <summary>
    /// Inventory class, used to hold the items of an actor
    /// </summary>
    public class Inventory
    {
        List<Item> inventory;

        public List<Item> Items { get { return inventory; } }
        public Weapon equipped;

        public Inventory()
        {
            inventory = new List<Item>();
        }

        public void Add(Item item)
        {
            if (item.Type != ItemType.Money)
            {
                inventory.Add(item);
            }
            else
            {
                Item gold = FindGold();
                if (gold == null)
                {
                    inventory.Add(item);
                    item.Name = String.Format("{0} gold pieces", item.Amount);
                }
                else
                {
                    gold.Amount += item.Amount;
                    gold.Name = String.Format("{0} gold pieces", gold.Amount);
                }
            }
        }

        public void Remove(Item item)
        {
            inventory.Remove(item);
        }

        public Item GetAt(int index)
        {
            return inventory[index];
        }

        public Item RemoveAt(int index)
        {
            Item itm = inventory[index];
            inventory.RemoveAt(index);
            return itm;
        }

        protected Item FindGold()
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].Type == ItemType.Money) return inventory[i];
            }
            return null;
        }
    }

    public abstract class Actor : Entity
    {
        protected Attributes attributes;
        protected Stats stats;
        protected Action action;
        protected string name;

        protected float healthRegenTimer;
        protected float manaRegenTimer;

        public Stats Stats { get { return stats; } }
        public Attributes Attributes { get { return attributes; } }
        public string Name { get { return name; } }

        public Inventory inventory;

        public Actor()
        {
        }

        public virtual void DoTurn()
        {
            healthRegenTimer += attributes.endurance / 55.0f;
            if (healthRegenTimer >= 1.0f)
            {
                AddHealth(this, 1);
                healthRegenTimer = 0;
            }

            manaRegenTimer += attributes.intelligence / 30.0f;
            if (manaRegenTimer >= 1.0f)
            {
                AddMana(this, 1);
                manaRegenTimer = 0;
            }
        }

        /// <summary>
        /// Updates the current action of the actor
        /// </summary>
        public override void Update(float timescale)
        {
            if (action != null)
            {
                if (!action.IsDone())
                {
                    action.Update(timescale);
                }
                else
                {
                    action = null;
                }
            }
        }

        public virtual void TakeDamage(Actor from, int dmgPoints)
        {
            stats.curHealth -= dmgPoints;
            if (stats.curHealth <= 0)
            {
                ownerMap.Gamestate.AddMessage(String.Format("The {0} dies.", name));
                ownerMap.entities.Remove(this);
            }
        }

        public virtual void AddHealth(Actor from, int points)
        {
            stats.curHealth += points;
            if (stats.curHealth > stats.maxHealth) stats.curHealth = stats.maxHealth;
        }
        

        public virtual void AddMana(Actor from, int points)
        {
            stats.curMana += points;
            if (stats.curMana > stats.maxMana) stats.curMana = stats.maxMana;
        }

        public virtual void MeleeAttack(Actor target)
        {
            if (attributes.CheckHit(target.Attributes))
            {
                int attack_damage = attributes.CheckMeleeDamage();

                if (inventory != null && inventory.equipped != null)
                    attack_damage += GameManager.GetInstance().Random.Next(1, inventory.equipped.Damage + 1);

                target.TakeDamage(this, attack_damage);
            }
            else
            {
                ownerMap.Gamestate.AddMessage(String.Format("The {0} attacks, but misses.", Name));
            }
        }

        /// <summary>
        /// Checks if the turn has completed
        /// </summary>
        public bool IsActionDone()
        {
            return (action == null);
        }
    }
}