using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RiotSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RIOT
{
    public partial class Form1 : Form
    {
        RiotSharp.RiotApi api;
        RiotSharp.StaticRiotApi staticApi;

        public Form1()
        {
            InitializeComponent();
            match_counter.Increment = 1;
            match_counter.Value = 0;

            string key = File.ReadAllText(@"D:\Riot\RIOT\RIOT\key_file.txt");

            api = RiotApi.GetInstance(key);
            staticApi = StaticRiotApi.GetInstance(key);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //@"D:\Riot\RIOT\RIOT\matches\5.11\NORMAL_5X5\", @"D:\Riot\RIOT\RIOT\matches\5.11\RANKED_SOLO\BR.json",, @"D:\Riot\RIOT\RIOT\matches\5.14\RANKED_SOLO\BR.json
            string[] folders = {@"D:\Riot\RIOT\RIOT\matches\5.14\NORMAL_5X5\"};
            var tuple_list = new List<Tuple<RiotSharp.Region, string>>();
            
            foreach (string folder in folders)
            {
                tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.br, folder + "BR.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.eune, folder + "EUNE.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.euw, folder + "EUW.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.kr, folder + "KR.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.lan, folder + "LAN.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.las, folder + "LAS.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.na, folder + "NA.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.oce, folder + "OCE.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.ru, folder + "RU.json"));
                //tuple_list.Add(new Tuple<RiotSharp.Region, string>(RiotSharp.Region.tr, folder + "TR.json"));
            };

            var match_list = new List<MatchWrapper>();
            long counter = 0;
            foreach (var tuple in tuple_list)
            {
                var region = tuple.Item1;
                string file_path = tuple.Item2;

                // This text is added only once to the file. 
                if (File.Exists(file_path))
                {
                    // Open the file to read from. 
                    string readText = File.ReadAllText(file_path);
                    List<long> list = JsonConvert.DeserializeObject<List<long>>(readText);

                    // For each match id in the given file, parse the match.
                    foreach (var match_id in list)
                    {
                        try
                        {
                            attempt_to_parse(region, match_id);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            counter++;
                            if (counter % 100 == 0)
                            {
                                Console.WriteLine("Counter: {0}", counter);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("File {0} does not exist! Skipping.", file_path);
                }
            }

            Console.WriteLine(" FINISHED LOADING THE CURRENT BATCH ");
        }

        public void attempt_to_parse(RiotSharp.Region region, long match_id, int attempt = 0)
        {
            var match = api.GetMatch(region, match_id, true);

            if (match == null && attempt < 5)
            {
                Console.WriteLine("Match {0} from {1} has failed {2} times!", match_id, region.ToString(), attempt);
                attempt++;
                attempt_to_parse(region, match_id, attempt);
                return;
            }
            else if (match == null)
            {
                Console.WriteLine("Failed to get match id {0} 5 times!", match_id);
                return;
            }

            MatchWrapper wrapper = new MatchWrapper(match, region.ToString());
            send_player_info(wrapper.player_rows);
            send_item_events(wrapper.item_rows_per_player);   
        }

        private void send_player_info(List<PlayerRow> player_rows)
        {
            MySQLConn myConn = new MySQLConn();
            string query = "insert into player_stats values ";

            for (int index = 0; index < player_rows.Count; index++)
            {
                var player_r = player_rows[index];
                if (index < player_rows.Count - 1)
                {
                    query += player_r.get_sql() + ",";
                }
                else
                {
                    query += player_r.get_sql() + ";";
                }
            }

            var cmd = new MySqlCommand(query, myConn.conn);
            cmd.ExecuteNonQuery();
            myConn.conn.Close();
        }

        private void send_item_events(Dictionary<int, List<ItemRow>> item_rows_per_player)
        {
            foreach (List<ItemRow> list in item_rows_per_player.Values)
            {
                MySQLConn myConn = new MySQLConn();
                string query = "insert into item_events values ";

                for (int index = 0; index < list.Count; index++)
                {
                    var item_event = list[index];
                    if (index < list.Count - 1)
                    {
                        query += item_event.get_sql() + ",";
                    }
                    else
                    {
                        query += item_event.get_sql() + ";";
                    }
                }
                var cmd = new MySqlCommand(query, myConn.conn);
                cmd.ExecuteNonQuery();

                myConn.conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string items_json = File.ReadAllText(@"D:\Riot\RIOT\RIOT\items.json");
            JObject o = JObject.Parse(items_json);
            var data = o["data"].Children<JProperty>();

            foreach (JProperty prop in data)
            {
                string id = (string)prop.Name;
                string name = o["data"][id]["name"].ToObject<string>();
                string description = o["data"][id]["description"].ToObject<string>();
                string img = o["data"][id]["image"]["full"].ToObject<string>();
                var tags = o["data"][id]["tags"].ToList().ConvertAll<string>(x => x.ToObject<string>());

                ItemDetailRow idr = new ItemDetailRow(id, name, description, img);
                idr.submit_to_db();

                tags.ForEach( tag => idr.submit_tag_row(tag) );
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string champs_json = File.ReadAllText(@"D:\Riot\RIOT\RIOT\champions.json");
            JObject o = JObject.Parse(champs_json);
            var data = o["data"].Children<JProperty>();

            foreach (JProperty prop in data)
            {
                string name = (string)prop.Name;
                string key = o["data"][name]["key"].ToObject<string>();
                string title = o["data"][name]["title"].ToObject<string>();
                string img = o["data"][name]["image"]["full"].ToObject<string>();
                var tags = o["data"][name]["tags"].ToList().ConvertAll<string>(x => x.ToObject<string>());

                ChampDetailRow idr = new ChampDetailRow(key, name, title, img);
                idr.submit_to_db();

                tags.ForEach(tag => idr.submit_tag_row(tag));
            }
        }
    }
}
