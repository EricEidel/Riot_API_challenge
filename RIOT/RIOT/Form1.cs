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

        public Form1()
        {
            InitializeComponent();
            StaticData.ParseItems();
            StaticData.ParseChampions();

            string key = File.ReadAllText(@"key_file.txt");
            api = RiotApi.GetInstance(key);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var region = RiotSharp.Region.na;
            attempt_to_parse(region, 2453622993);           
        }

        public void attempt_to_parse(RiotSharp.Region region, long match_id, int attempt = 0)
        {
            var match = api.GetMatch(region, match_id, true);

            MatchWrapper wrapper = new MatchWrapper(match, region.ToString());
            Console.WriteLine(wrapper.ToString());
        }

    }
}
