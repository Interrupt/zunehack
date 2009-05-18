using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ZuneHack.GUI;

namespace ZuneHack
{
    class InventoryState : GameState
    {
        Texture2D bg;
        Menu mnu_actions;
        Menu mnu_inv;
        Player player;
        bool onSecondaryList;
        string secondaryTitle;

        public InventoryState(Player Player, GameManager manager) : base(manager)
        {
            player = Player;

            mnu_actions = new Menu();
            mnu_actions.AddItem("Inventory", "inventory");
            mnu_actions.AddItem("Equip", "equip");
            mnu_actions.AddItem("Use", "use");
            mnu_actions.AddItem("Drop", "drop");
            mnu_actions.AddItem("Read", "read");

            onSecondaryList = false;
        }

        public override void End()
        {

        }

        public override void Update(float timescale)
        {

        }

        public void MakeInventoryList(string filter)
        {
            mnu_inv = new Menu();
            List<Item> inv = player.inventory.Items;

            if (filter == "weapons")
                inv = inv.Where(p => p.Type == ItemType.Weapon).ToList();
            else if (filter == "potions")
                inv = inv.Where(p => p.Type == ItemType.Potion).ToList();
            else if (filter == "scrolls")
                inv = inv.Where(p => p.Type == ItemType.Scroll).ToList();
            else if (filter == "food")
                inv = inv.Where(p => p.Type == ItemType.Food).ToList();

            for (int i = 0; i < inv.Count; i++)
            {
                mnu_inv.AddItem(inv[i].Name, inv[i]);
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            // Draws the menu
            Vector2 drawPos = new Vector2(20, 20);
            batch.DrawString(manager.Font, "Actions", drawPos, Color.SkyBlue);
            drawPos.Y += 20;

            mnu_actions.DrawList(manager.Font, batch, drawPos, new Vector2(0, 20));

            if(onSecondaryList)
            {
                drawPos = new Vector2(100, 20);
                batch.DrawString(manager.Font, secondaryTitle, drawPos, Color.SkyBlue);
                drawPos.Y += 20;

                mnu_inv.DrawList(manager.Font, batch, drawPos, new Vector2(0, 20));
            }
        }

        public override void Input(InputStates input)
        {
            if (input.IsNewButtonPress(Buttons.A) || input.IsNewKeyPress(Keys.Space))
            {
                if (!onSecondaryList)
                {
                    MenuItem selected = mnu_actions.GetSelectedItem();
                    if (selected != null)
                    {
                        if ((string)selected.value == "inventory")
                        {
                            onSecondaryList = true;
                            MakeInventoryList("all");
                            secondaryTitle = "Your Items";
                        }
                        else if ((string)selected.value == "equip")
                        {
                            onSecondaryList = true;
                            MakeInventoryList("weapons");
                            secondaryTitle = "Equip Which?";
                        }
                        else if ((string)selected.value == "use")
                        {
                            onSecondaryList = true;
                            MakeInventoryList("potions");
                            secondaryTitle = "Use Which?";
                        }
                        else if ((string)selected.value == "drop")
                        {
                            onSecondaryList = true;
                            MakeInventoryList("all");
                            secondaryTitle = "Drop Which?";
                        }
                        else if ((string)selected.value == "read")
                        {
                            onSecondaryList = true;
                            MakeInventoryList("scrolls");
                            secondaryTitle = "Read Which?";
                        }
                    }
                }
                else
                {
                    string action = (string)mnu_actions.GetSelectedItem().value;
                    MenuItem selected = mnu_inv.GetSelectedItem();

                    if (action == "use")
                        Use(((Item)selected.value));
                    else if (action == "drop")
                        Drop(((Item)selected.value));
                    else if (action == "read")
                        Read(((Item)selected.value));
                    else if (action == "equip")
                        Equip(((Item)selected.value));
                    else if (action == "inventory")
                        View(((Item)selected.value));
                }
            }

            if (input.IsNewButtonPress(Buttons.DPadRight) || input.IsNewKeyPress(Keys.Down))
            {
                if (onSecondaryList)
                {
                    mnu_inv.Select(mnu_inv.GetSelectedIndex() + 1);
                }
                else
                {
                    mnu_actions.Select(mnu_actions.GetSelectedIndex() + 1);
                }
            }

            if (input.IsNewButtonPress(Buttons.DPadLeft) || input.IsNewKeyPress(Keys.Up))
            {
                if (onSecondaryList)
                {
                    mnu_inv.Select(mnu_inv.GetSelectedIndex() - 1);
                }
                else
                {
                    mnu_actions.Select(mnu_actions.GetSelectedIndex() - 1);
                }
            }

            if (input.IsNewButtonPress(Buttons.B) || input.IsNewKeyPress(Keys.Enter)
                || input.IsNewButtonPress(Buttons.Back) || input.IsNewKeyPress(Keys.Escape))
            {
                if (onSecondaryList) onSecondaryList = false;
                else
                {
                    manager.PopState();
                }
            }
        }

        public void Use(Item itm)
        {
            if (itm.Type == ItemType.Potion)
            {
                player.AddHealth(player, 4);
                player.inventory.Remove(itm);

                player.GetMap().Gamestate.AddMessage("You quaff the potion");
                player.GetMap().Gamestate.AddMessage("You feel better");

                manager.PopState();
            }
        }

        public void Drop(Item itm)
        {
            itm.pos = new Vector2(player.MapPosX, player.MapPosY) + new Vector2(0.5f, 0.5f);
            player.GetMap().AddEntity(itm);
            player.inventory.Remove(itm);

            string message = "You drop a ";
            if(itm.Amount > 1) message = "You drop ";
            player.GetMap().Gamestate.AddMessage(message + itm.Name);

            manager.PopState();
        }

        public void Read(Item itm)
        {

        }

        public void Equip(Item itm)
        {

        }

        public void View(Item itm)
        {

        }
    }
}
