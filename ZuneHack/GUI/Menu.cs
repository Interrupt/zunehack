using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ZuneHack.GUI
{
    class MenuItem
    {
        public string text;
        public object value;
        public bool selected;
    }

    class Menu
    {
        protected List<MenuItem> items;
        protected int selectedNum;
        protected int visibleLength;
        protected int scrollPos = 0;

        public Menu()
        {
            visibleLength = 9;
        }

        public Menu(int numShown)
        {
            visibleLength = numShown;
        }

        public void AddItem(string text, object value)
        {
            MenuItem item = new MenuItem();
            item.text = text;
            item.value = value;
            item.selected = false;

            if (items == null)
            {
                items = new List<MenuItem>();
                item.selected = true;
            }

            items.Add(item);
        }

        public List<MenuItem> GetMenuItems()
        {
            return items;
        }

        public void RemoveItem(int index)
        {
            if (selectedNum > index) Select(selectedNum--);
            if (items != null) items.RemoveAt(index);
        }

        public int Count()
        {
            if (items != null) return items.Count;
            return 0;
        }

        public MenuItem GetItemAt(int index)
        {
            if (index < items.Count)
                return items[index];
            return null;
        }

        public void Select(int index)
        {
            if(items != null)
            {
                GetSelectedItem().selected = false;

                if (index > items.Count - 1)
                    index = 0;
                else if (index < 0)
                    index = items.Count - 1;

                items[index].selected = true;
                selectedNum = index;
            }
        }

        public int GetSelectedIndex()
        {
            return selectedNum;
        }

        public MenuItem GetSelectedItem()
        {
            if (selectedNum < items.Count)
            {
                return items[selectedNum];
            }
            return null;
        }

        /// <summary>
        /// Draws the menu as a simple list
        /// </summary>
        public void DrawList(SpriteFont font, SpriteBatch batch, Vector2 pos, Vector2 spacing)
        {
            if (items == null) return;

            if (items.Count > visibleLength)
            {
                if (selectedNum > (visibleLength - 1) + scrollPos) scrollPos++;
                else if (selectedNum < scrollPos) scrollPos--;
            }

            for (int i = scrollPos; i < scrollPos + visibleLength && i < items.Count; i++)
            {
                MenuItem item = items[i];
                if (item != null)
                {
                    batch.DrawString(font, item.text, pos, item.selected == true ? Color.White : Color.DarkGray);
                    pos += spacing;
                }
            }
        }
    }
}
