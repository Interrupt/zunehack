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
        Menu mnu_inv;
        Player player;

        bool DownWasPressed;
        bool UpWasPressed;

        public InventoryState(Player Player, GameManager manager) : base(manager)
        {
            player = Player;
            mnu_inv = new Menu();
            Inventory inv = player.inventory;
            for (int i = 0; i < inv.Items.Count; i++)
            {
                mnu_inv.AddItem(inv.Items[i].Name, inv.Items[i]);
            }
        }

        public override void End()
        {

        }

        public override void Update(float timescale)
        {

        }

        public override void Draw(SpriteBatch batch)
        {
            if (mnu_inv == null) return;

            // Draws the menu
            Vector2 drawPos = new Vector2(20, 20);
            batch.DrawString(manager.Font, "Your Inventory", drawPos, Color.SkyBlue);
            drawPos.Y += 20;

            int numItems = mnu_inv.Count();
            for (int i = 0; i < numItems; i++)
            {
                MenuItem item = mnu_inv.GetItemAt(i);
                if (item != null)
                {
                    batch.DrawString(manager.Font, item.text, drawPos, item.selected == true ? Color.White : Color.DarkGray);
                    drawPos.Y += 20;
                }
            }
        }

        public override void Input(GamePadState gamepadState, KeyboardState keyState)
        {
            if (gamepadState.Buttons.A == ButtonState.Pressed || keyState.IsKeyDown(Keys.Space))
            {
                if (mnu_inv.Count() < 1)
                {
                    manager.PopState();
                    return;
                }

                MenuItem selected = mnu_inv.GetSelectedItem();
                if (selected != null)
                {
                    Item itm = ((Item)selected.value);
                    if (itm.Type == ItemType.Potion)
                    {
                        player.inventory.Remove(itm);

                        player.AddHealth(player, itm.Amount);

                        player.GetMap().Gamestate.AddMessage("You quaff the potion");
                        player.GetMap().Gamestate.AddMessage("You feel better");
                    }

                    manager.PopState();
                }
            }

            if (gamepadState.DPad.Right == ButtonState.Pressed || keyState.IsKeyDown(Keys.Down))
            {
                if (!UpWasPressed)
                {
                    UpWasPressed = true;
                    mnu_inv.Select(mnu_inv.GetSelectedIndex() + 1);
                }
            }
            else
            {
                UpWasPressed = false;
            }

            if (gamepadState.DPad.Left == ButtonState.Pressed || keyState.IsKeyDown(Keys.Up))
            {
                if (!DownWasPressed)
                {
                    DownWasPressed = true;
                    mnu_inv.Select(mnu_inv.GetSelectedIndex() - 1);
                }
            }
            else
            {
                DownWasPressed = false;
            }

            if (gamepadState.Buttons.B == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
            {
                manager.PopState();
            }
        }
    }
}
