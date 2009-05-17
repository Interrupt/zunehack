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
    class IntroState : GameState
    {
        Texture2D bg;

        bool DownWasPressed;
        bool UpWasPressed;

        Menu mainMenu;

        public IntroState(GameManager manager) : base(manager)
        {
            bg = manager.LoadTexture("title");

            mainMenu = new Menu();
            mainMenu.AddItem("New Adventurer", "new");
            //mainMenu.AddItem("Continue", "continue");
            mainMenu.AddItem("Quit", "quit");
        }

        public override void End()
        {

        }

        public override void Update(float timescale)
        {

        }

        public override void Draw(SpriteBatch batch)
        {
            // Draws the background image
            batch.Draw(bg, new Rectangle(0, 0, 320, 240), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 1);

            if (mainMenu == null) return;

            // Draws the menu
            Vector2 drawPos = new Vector2(20, 170);
            int numItems = mainMenu.Count();
            for (int i = 0; i < numItems; i++)
            {
                MenuItem item = mainMenu.GetItemAt(i);
                if (item != null)
                {
                    batch.DrawString(manager.Font, item.text, drawPos, item.selected == true ? Color.White : Color.DarkGray);
                    drawPos.Y += 20;
                }
            }
        }

        public override void Input(InputStates input)
        {
            if (input.IsNewButtonPress(Buttons.A) || input.IsNewKeyPress(Keys.Space))
            {
                MenuItem selected = mainMenu.GetSelectedItem();
                if (selected != null)
                {
                    if ((string)selected.value == "new")
                    {
                        manager.PopState();
                        manager.PushState(new PlayState(manager));
                    }
                    else if ((string)selected.value == "continue")
                    {
                        // TODO: Implement loading a game to continue
                    }
                    else if ((string)selected.value == "quit")
                    {
                        manager.Quit(0);
                    }
                }
            }

            if (input.IsNewButtonPress(Buttons.DPadRight) || input.IsNewKeyPress(Keys.Down))
            {
                mainMenu.Select(mainMenu.GetSelectedIndex() + 1);
            }

            if (input.IsNewButtonPress(Buttons.DPadLeft) || input.IsNewKeyPress(Keys.Up))
            {
                mainMenu.Select(mainMenu.GetSelectedIndex() - 1);
            }
        }
    }
}
