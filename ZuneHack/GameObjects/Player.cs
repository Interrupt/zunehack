using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZuneHack
{
    public enum PlayerInput
    {
        forward,
        backward,
        left,
        right,
        button,
    }

    public class Player : Actor
    {
        protected bool turnDone;
        protected Camera camera;

        public bool IsTurnDone()
        {
            return turnDone && IsActionDone();
        }

        public Player(Camera cam)
        {
            // The camera for the player to look out of
            camera = cam;

            // Bam, average player.
            attributes.strength = 5;
            attributes.endurance = 5;
            attributes.agility = 6;
            attributes.intelligence = 5;
            attributes.speed = 5;
            attributes.constitution = 6;

            stats.Initialize(1, attributes);

            // TODO: Give the player some random starting items
            inventory = new Inventory();

            inventory.Add(ItemCreator.CreateGold(10));
            inventory.Add(new Potion(PotionType.Health));

            turnDone = false;
        }

        public void EndTurn()
        {
            turnDone = true;
            DoTurn();
        }

        public void StartTurn()
        {
            turnDone = false;
        }

        public void TurnInput(PlayerInput input)
        {
            ownerMap.Gamestate.didTurnAction();

            if (input == PlayerInput.right)
            {
                action = new PlayerTurnAction(0.2f, -1, camera);
            }
            else if (input == PlayerInput.left)
            {
                action = new PlayerTurnAction(0.2f, 1, camera);
            }
            else if (input == PlayerInput.backward)
            {
                if (!ownerMap.checkMovability(pos - dir))
                {
                    action = new PlayerMoveAction(0.2f, pos - dir, camera);
                }
                else
                {
                    action = new PlayerPauseAction(0.4f);
                    ownerMap.Gamestate.AddMessage("Ouch!");
                }
                EndTurn();
            }
            else if (input == PlayerInput.forward)
            {
                action = new PlayerMoveAction(0.2f, pos + dir, camera);

                if (!ownerMap.checkMovability(pos + dir))
                {
                    action = new PlayerMoveAction(0.2f, pos + dir, camera);
                }
                else
                {
                    Entity toAttack = ownerMap.checkEntityHit(pos + dir);
                    if (toAttack as Actor != null)
                    {
                        action = new PlayerMeleeAction(0.10f);
                        MeleeAttack((Actor)toAttack);
                    }
                    else if (toAttack as Door != null)
                    {
                        (toAttack as Door).Toggle(0.16f);
                        action = new PlayerPauseAction(0.2f);
                    }
                    else
                    {
                        action = new PlayerPauseAction(0.4f);
                        ownerMap.Gamestate.AddMessage("Ouch!");
                    }
                }
                EndTurn();
            }
            else if (input == PlayerInput.button)
            {
                int tileType = ownerMap.GetTileAt(MapPosX, MapPosY);
                Item itemHit = ownerMap.GetItemAt(new Vector2(MapPosX, MapPosY));

                if (tileType == -2)
                {
                    ownerMap.Gamestate.AddMessage("You climb down the ladder");
                    ownerMap.Gamestate.GoDownLevel();
                }
                else if (itemHit != null)
                {
                    string message = "You pick up a ";
                    if (itemHit.Amount > 1) message = "You pick up ";

                    ownerMap.Gamestate.AddMessage(message + itemHit.Name);
                    inventory.Add(itemHit);
                    ownerMap.entities.Remove(itemHit);
                    action = new PlayerPauseAction(0.4f);
                }
                else
                {
                    ownerMap.Gamestate.AddMessage("You see nothing here");
                }

                action = new PlayerPauseAction(0.4f);
                EndTurn();
            }
        }

        public override void MeleeAttack(Actor target)
        {
            if (attributes.CheckHit(target.Attributes))
            {
                int attack_damage = attributes.CheckMeleeDamage();

                if (inventory != null && inventory.equipped != null)
                    attack_damage += GameManager.GetInstance().Random.Next(1, inventory.equipped.Damage + 1);

                ownerMap.Gamestate.AddMessage(String.Format("You hit the {0} for {1} damage.", target.Name, attack_damage));
                target.TakeDamage(this, attack_damage);
            }
            else
            {
                ownerMap.Gamestate.AddMessage(String.Format("Your attack misses the {0}", target.Name));
            }
        }

        public override void TakeDamage(Actor from, int dmgPoints)
        {
            if(from != this)
                ownerMap.Gamestate.AddMessage(String.Format("The {0} attacks for {1} damage.", from.Name, dmgPoints));

            stats.curHealth -= dmgPoints;
            if (stats.curHealth <= 0)
            {
                 ownerMap.Gamestate.AddMessage("You die.");
                GameManager.GetInstance().Quit(100);
            }
        }

        public override void Update(float timescale)
        {
            base.Update(timescale);
        }
    }
}