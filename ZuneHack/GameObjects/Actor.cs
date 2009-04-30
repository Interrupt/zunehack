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

        public bool CheckHit()
        {
            return new Random().Next(1, 12) <= agility;
        }

        public int CheckMeleeDamage()
        {
            return new Random().Next(1, strength);
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

    public abstract class Actor : Entity
    {
        protected Attributes attributes;
        protected Stats stats;
        protected Action action;
        protected string name;

        public Stats Stats { get { return stats; } }
        public Attributes Attributes { get { return attributes; } }
        public string Name { get { return name; } }

        public Actor()
        {
        }

        public virtual void DoTurn()
        {
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
                GameManager.GetInstance().AddMessage(String.Format("The {0} dies.", name));
                GameManager.GetInstance().Map.entities.Remove(this);
            }
        }

        public virtual void MeleeAttack(Actor target)
        {
            if (attributes.CheckHit())
            {
                target.TakeDamage(this, attributes.CheckMeleeDamage());
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