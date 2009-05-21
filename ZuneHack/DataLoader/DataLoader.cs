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
        public static List<MonsterData> LoadMonsters()
        {
            string path = StorageContainer.TitleLocation + @"\Data\monsters.json";
            System.IO.TextReader reader = new System.IO.StreamReader(path);
            List<MonsterData> monsters = new List<MonsterData>();

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
                        if (cur != null) monsters.Add(MonsterFromHash(cur));
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

            // Add the last monster
            if (cur != null) monsters.Add(MonsterFromHash(cur));

            return monsters;
        }

        protected static MonsterData MonsterFromHash(Hashtable data)
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

        public static List<WeaponData> LoadWeapons()
        {
            string path = StorageContainer.TitleLocation + @"\Data\weapons.json";
            System.IO.TextReader reader = new System.IO.StreamReader(path);
            List<WeaponData> list = new List<WeaponData>();

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
                        if (cur != null) list.Add(WeaponFromHash(cur));
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

            // Add the last monster
            if (cur != null) list.Add(WeaponFromHash(cur));

            return list;
        }

        protected static WeaponData WeaponFromHash(Hashtable data)
        {
            WeaponData itm = new WeaponData();
            itm.name = (string)data["name"];
            itm.image = (string)data["image"];

            itm.level = Convert.ToInt32(data["lvl"]);
            itm.damage = Convert.ToInt32(data["damage"]);

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
            }

            return itm;
        }
    }
}
