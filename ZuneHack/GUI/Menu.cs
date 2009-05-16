using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
