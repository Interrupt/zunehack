using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using System.Collections;
using Newtonsoft.Json;
using ZuneHack;

namespace ZuneHack.DataLoader
{
    class Loader
    {
        /// <summary>
        /// Performs an action for each record in the list
        /// </summary>
        public static void ForEachRecord(List<Hashtable> records, Action<Hashtable> action)
        {
            for (int i = 0; i < records.Count; i++)
            {
                action.Invoke(records[i]);
            }
        }

        /// <summary>
        /// Loads a list from a json file and returns a hashtable for each record
        /// </summary>
        public static List<Hashtable> RecordsFromFile(string file)
        {
            string path = StorageContainer.TitleLocation + file;
            System.IO.TextReader reader = new System.IO.StreamReader(path);
            List<Hashtable> list = new List<Hashtable>();

            Hashtable cur = null;

            // Read through each object in the file
            JsonTextReader json = new JsonTextReader(reader);
            while (json.Read())
            {
                if (json.Value == null && (json.TokenType == JsonToken.StartObject || json.TokenType == JsonToken.EndObject))
                {
                    json.Read();
                    if (json.TokenType == JsonToken.PropertyName)
                    {
                        if (cur != null) list.Add(cur);
                        cur = new Hashtable();
                        cur["name"] = (string)json.Value;
                        json.Read();
                    }
                }
                else if (json.TokenType == JsonToken.PropertyName)
                {
                    string val = (string)json.Value;
                    json.Read();
                    cur[val] = json.Value;
                }
            }

            // Add the last record
            if (cur != null) list.Add(cur);

            json.Close();
            return list;
        }

        public static List<MonsterData> LoadMonsters()
        {
            List<Hashtable> records = RecordsFromFile(@"\Data\monsters.json");
            List<MonsterData> data = new List<MonsterData>();
            ForEachRecord(records, i => data.Add(MonsterDataFromHash(i)));
            return data;
        }

        public static List<WeaponData> LoadWeapons()
        {
            List<Hashtable> records = RecordsFromFile(@"\Data\weapons.json");
            List<WeaponData> data = new List<WeaponData>();
            ForEachRecord(records, i => data.Add(WeaponDataFromHash(i)));
            return data;
        }

        public static List<ArmorData> LoadArmor()
        {
            List<Hashtable> records = RecordsFromFile(@"\Data\armor.json");
            List<ArmorData> data = new List<ArmorData>();
            ForEachRecord(records, i => data.Add(ArmorDataFromHash(i)));
            return data;
        }

        // --------------- Data Records From Hash Functions ---------------- //

        protected static MonsterData MonsterDataFromHash(Hashtable data)
        {
            MonsterData mon = new MonsterData();
            mon.name = (string)data["name"];
            mon.image = (string)data["image"];
            mon.level = Convert.ToInt32(data["lvl"]);

            mon.attribs.agility = Convert.ToInt32(data["agility"]);
            mon.attribs.constitution = Convert.ToInt32(data["constitution"]);
            mon.attribs.endurance = Convert.ToInt32(data["endurance"]);
            mon.attribs.intelligence = Convert.ToInt32(data["intelligence"]);
            mon.attribs.speed = Convert.ToInt32(data["speed"]);
            mon.attribs.strength = Convert.ToInt32(data["strength"]);

            return mon;
        }

        protected static WeaponData WeaponDataFromHash(Hashtable data)
        {
            WeaponData itm = new WeaponData();
            itm.name = (string)data["name"];
            itm.image = (string)data["image"];

            itm.level = Convert.ToInt32(data["lvl"]);
            itm.damage = Convert.ToInt32(data["damage"]);
            itm.bonus = Convert.ToInt32(data["bonus"]);
            itm.chance = data["chance"] != null ? Convert.ToInt32(data["chance"]) : 10;

            switch (((string)data["class"]).ToLower())
            {
                case "axe":
                    itm.wpnclass = WeaponClass.Axe;
                    break;
                case "blade":
                    itm.wpnclass = WeaponClass.Blade;
                    break;
                case "blunt":
                    itm.wpnclass = WeaponClass.Blunt;
                    break;
                case "piercing":
                    itm.wpnclass = WeaponClass.Piercing;
                    break;
                default:
                    itm.wpnclass = WeaponClass.Blade;
                    break;
            }

            return itm;
        }

        protected static ArmorData ArmorDataFromHash(Hashtable data)
        {
            ArmorData itm = new ArmorData();
            itm.name = (string)data["name"];
            itm.image = (string)data["image"];

            itm.level = Convert.ToInt32(data["lvl"]);
            itm.armor = Convert.ToInt32(data["armor"]);
            itm.bonus = Convert.ToInt32(data["bonus"]);
            itm.chance = data["chance"] != null ? Convert.ToInt32(data["chance"]) : 10;

            switch (((string)data["location"]).ToLower())
            {
                case "arms":
                    itm.slot = ArmorBodySlot.Arms;
                    break;
                case "body":
                    itm.slot = ArmorBodySlot.Body;
                    break;
                case "feet":
                    itm.slot = ArmorBodySlot.Feet;
                    break;
                case "head":
                    itm.slot = ArmorBodySlot.Head;
                    break;
                case "offhand":
                    itm.slot = ArmorBodySlot.Offhand;
                    break;
                default:
                    itm.slot = ArmorBodySlot.Body;
                    break;
            }

            return itm;
        }
    }
}
