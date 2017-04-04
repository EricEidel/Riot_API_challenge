using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RIOT.Containers;
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
    /***
     * To add MySQL reference to the project, follow: http://stackoverflow.com/questions/1102281/how-do-i-add-a-reference-to-the-mysql-connector-for-net
     */

    public partial class Form1 : Form
    {
        RiotSharp.RiotApi api;

        DateTime cutoff_timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1490054400000).DateTime; // 21th of March 2017, patch 7.6

        public Form1()
        {
            InitializeComponent();
            StaticData.ParseItems();
            StaticData.ParseChampions();

            string key = File.ReadAllText(@"key_file.txt");
            api = RiotApi.GetInstance(key);
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            var region = RiotSharp.Region.na;
            long summoner_id = 24727186; // ThuluTheAmazing

            get_matches_for_summoner_id(region, summoner_id);
        }

        public void get_matches_for_summoner_id(RiotSharp.Region region, long summoner_id, int attempt = 0)
        {
            var match_list = api.GetMatchList(region, summoner_id);

            int index = 0;
            var match = match_list.Matches[index];

            while (match.Timestamp > cutoff_timestamp)
            {
                Console.WriteLine("Match timestamp: {0} , Match ID: {1}", match.Timestamp, match.MatchID);

                index++;
                match = match_list.Matches[index];
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var region = RiotSharp.Region.na;
            parse_match(region, 2465557406);
            parse_match(region, 2464714106);
            parse_match(region, 2463850080);
        }

        public void parse_match(RiotSharp.Region region, long match_id, int attempt = 0)
        {
            var match = api.GetMatch(region, match_id, true);

            MatchWrapper wrapper = new MatchWrapper(match, region.ToString());
            Console.WriteLine(wrapper.ToString());
        }
    }
}
