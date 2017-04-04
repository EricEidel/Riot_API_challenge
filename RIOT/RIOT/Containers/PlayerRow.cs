using RIOT.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT
{
    public class PlayerRow
    {
        // Match info
        public string match_version;
        public long match_id;
        public string queue_type;
        public string region;
        public long? summoner_id;
        public string summoner_name = "";

        // General champ/summoner info
        public int champ_id;
        public int part_index;
        public bool is_winner;
        public RiotSharp.Lane lane;

        // Battle stats
        long magic_damage_dealt;
        long magic_damage_dealt_champs;
        long damage_dealt;
        long damage_dealt_champs;
        long healing_done;

        // Farming stats
        long minions_killed;
        long gold_earned;

        // KDA stats
        long kills;
        long deaths;
        long assists;

        // Wards
        long wards_killed;
        long wards_placed;

        public PlayerRow(string match_version, long match_id, string region, string queue_type, RiotSharp.MatchEndpoint.Participant participant, RiotSharp.MatchEndpoint.ParticipantIdentity ident)
        {
            // Match info
            this.match_version = match_version;
            this.match_id = match_id;
            this.queue_type = queue_type;
            this.region = region;
            if (ident.Player != null)
            { 
                this.summoner_id = ident.Player.SummonerId;
                this.summoner_name = ident.Player.SummonerName;
            }

            // Get participant and participant identity for this player
            RiotSharp.MatchEndpoint.ParticipantStats stat  = participant.Stats;

            // General cham/summoner info
            this.champ_id = participant.ChampionId;
            this.part_index = participant.ParticipantId;
            this.is_winner = stat.Winner;
            this.lane = participant.Timeline.Lane;

            // Battle stats
            this.magic_damage_dealt = stat.MagicDamageDealt;
            this.magic_damage_dealt_champs = stat.MagicDamageDealtToChampions;
            this.damage_dealt = stat.TotalDamageDealt;
            this.damage_dealt_champs = stat.TotalDamageDealtToChampions;
            this.healing_done = stat.TotalHeal;

            // Farming stats
            this.minions_killed = stat.MinionsKilled;
            this.gold_earned = stat.GoldEarned;

            // KDA stats
            this.kills = stat.Kills;
            this.deaths = stat.Deaths;
            this.assists = stat.Assists;

            // Wards
            this.wards_killed = stat.WardsKilled;
            this.wards_placed = stat.WardsPlaced;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Summoner ID (name): {0} ({1})", summoner_id, summoner_name);
            sb.AppendLine();
            sb.AppendFormat("Champion (name): {0} ({1})", champ_id, StaticData.champions[champ_id].name);
            sb.AppendLine();
            sb.AppendFormat("Did player win? {0}", is_winner);
            sb.AppendLine(); 
            sb.AppendFormat("Player lane: {0}", lane);
            sb.AppendLine(); 
            sb.AppendFormat("Total Damage Delt: {0} (Champs: {1})\n" +
                            "Magic Damage Delt: {2} (Champs: {3})\n" +
                            "Total Healing: {4}",
                            damage_dealt, damage_dealt_champs, magic_damage_dealt, magic_damage_dealt_champs, healing_done);
            sb.AppendLine();
            sb.AppendFormat("Gold: {0}, CS: {1}", gold_earned, minions_killed);
            sb.AppendLine();
            sb.AppendFormat("K: {0}, D: {1}, A: {2}", kills, deaths, assists);
            sb.AppendLine(); 
            sb.AppendFormat("Wards (Placed/Killed): {0} / {1}", wards_placed, wards_killed);
            sb.AppendLine();

            return sb.ToString();
        }

        public string get_sql()
        {
            string summoner_id_str = "";
            if (this.summoner_id != null)
            {
                summoner_id_str = summoner_id.ToString();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append("'" + this.match_version + "', ");
            sb.Append("'" + this.match_id + "', ");
            sb.Append("'" + this.queue_type + "', ");
            sb.Append("'" + this.region + "', ");
            sb.Append("'" + this.part_index + "', ");
            sb.Append("'" + this.champ_id + "', ");

            string temp = this.is_winner.ToString();
            sb.Append("'" + temp + "', ");
            sb.Append("'" + this.damage_dealt + "', ");
            sb.Append("'" + this.damage_dealt_champs + "', ");
            sb.Append("'" + this.magic_damage_dealt + "', ");
            sb.Append("'" + this.magic_damage_dealt_champs + "', ");
            sb.Append("'" + this.healing_done + "', ");
            sb.Append("'" + this.minions_killed + "', ");
            sb.Append("'" + this.gold_earned + "', ");
            sb.Append("'" + this.kills + "', ");
            sb.Append("'" + this.deaths + "', ");
            sb.Append("'" + this.deaths + "',");
            sb.Append("'" + summoner_id_str + "', ");
            sb.Append("'" + this.summoner_name + "'");
            sb.Append(")");

            return sb.ToString();
        }
    }
}
