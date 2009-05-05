using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZuneHack
{
    enum PlayerInput
    {
        forward,
        backward,
        left,
        right,
        button
    }

    class Player : Actor
    {
        protected bool turnDone;

        public bool IsTurnDone()
        {
            return turnDone && IsActionDone();
        }

        public Player(Vector2 StartPos)
        {
            // Bam, average player.
            attributes.strength = 5;
            attributes.endurance = 5;
            attributes.agility = 6;
            attributes.intelligence = 5;
            attributes.speed = 5;
            attributes.constitution = 5;

            stats.Initialize(1, attributes);

            pos = StartPos;
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
            GameManager.GetInstance().didTurnAction();

            if (input == PlayerInput.right)
            {
                action = new PlayerTurnAction(0.2f, -1, GameManager.GetInstance().Camera);
            }
            else if (input == PlayerInput.left)
            {
                action = new PlayerTurnAction(0.2f, 1, GameManager.GetInstance().Camera);
            }
            else if (input == PlayerInput.backward)
            {
                if (!GameManager.GetInstance().Map.checkMovability(pos - dir))
                {
                    action = new PlayerMoveAction(0.2f, pos - dir, GameManager.GetInstance().Camera);
                }
                else
                {
                    action = new PlayerPauseAction(0.4f);
                    GameManager.GetInstance().AddMessage("Ouch!");
                }
                EndTurn();
            }
            else if (input == PlayerInput.forward)
            {
                action = new PlayerMoveAction(0.2f, pos + dir, GameManager.GetInstance().Camera);

                if (!GameManager.GetInstance().Map.checkMovability(pos + dir))
                {
                    action = new PlayerMoveAction(0.2f, pos + dir, GameManager.GetInstance().Camera);
                }
                else
                {
                    Entity toAttack = GameManager.GetInstance().Map.checkEntityHit(pos + dir);
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
                        GameManager.GetInstance().AddMessage("Ouch!");
                    }
                }
                EndTurn();
            }
            else if (input == PlayerInput.button)
            {
                int tileType = GameManager.GetInstance().Map.GetTileAt(MapPosX, MapPosY);
                Item itemHit = GameManager.GetInstance().Map.GetItemAt(new Vector2(MapPosX, MapPosY));

                if (tileType == -2)
                {
                    GameManager.GetInstance().AddMessage("You climb down the ladder");
                    GameManager.GetInstance().GoDownLevel();
                }
                else if (itemHit != null)
                {
                    GameManager.GetInstance().AddMessage("You pick up a " + itemHit.Name);
                    GameManager.GetInstance().Map.entities.Remove(itemHit);
                    action = new PlayerPauseAction(0.4f);
                }
                else
                {
                    GameManager.GetInstance().AddMessage("You see nothing here");
                }

                action = new PlayerPauseAction(0.4f);
                EndTurn();
            }
        }

        public override void MeleeAttack(Actor target)
        {
            if (attributes.CheckHit(target.Attributes))
            {
                int dmgPoints = attributes.CheckMeleeDamage();
                GameManager.GetInstance().AddMessage(String.Format("You hit the {0} for {1} damage.", target.Name, dmgPoints));
                target.TakeDamage(this, dmgPoints);
            }
            else
            {
                GameManager.GetInstance().AddMessage(String.Format("Your attack misses the {0}", target.Name));
            }
        }

        public override void TakeDamage(Actor from, int dmgPoints)
        {
            GameManager.GetInstance().AddMessage(String.Format("The {0} attacks for {1} damage.", from.Name, dmgPoints));

            stats.curHealth -= dmgPoints;
            if (stats.curHealth <= 0)
            {
                GameManager.GetInstance().AddMessage("You die.");
                GameManager.GetInstance().Quit();
            }
        }

        public override void Update(float timescale)
        {
            base.Update(timescale);
        }
    }
}