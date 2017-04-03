using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT.Containers
{
    public static class StaticData
    {
        public static Dictionary<int, Item> items = new Dictionary<int, Item>();
        public static Dictionary<int, Champion> champions = new Dictionary<int, Champion>();

        static public void ParseItems()
        {
            var data = System.IO.File.ReadAllText(@"C:\Users\Eric\Documents\Riot_API_challenge\RIOT\RIOT\items.json");

            JObject items_str = JObject.Parse(data);
            var items_data = items_str["data"] as JObject;

            foreach (var token in items_data)
            {
                string name = items_str["data"][token.Key]["name"].Value<string>();
                int id = int.Parse(token.Key);

                items.Add(id, new Item(id, name));
            }

            Console.WriteLine("Parsed " + items.Count + " items from items.json.");
        }

        static public void ParseChampions()
        {
            var data = System.IO.File.ReadAllText(@"C:\Users\Eric\Documents\Riot_API_challenge\RIOT\RIOT\champions.json");

            JObject champ_str = JObject.Parse(data);
            var champ_data = champ_str["data"] as JObject;

            foreach (var token in champ_data)
            {
                string name = champ_str["data"][token.Key]["name"].Value<string>(); 
                string key = champ_str["data"][token.Key]["key"].Value<string>();
                int id = int.Parse(key);

                champions.Add(id, new Champion(name, id));
            }

            Console.WriteLine("Parsed " + champions.Count + " champions from champions.json.");
        }
    }
}
