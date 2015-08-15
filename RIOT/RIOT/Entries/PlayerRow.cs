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
        string match_version;
        long match_id;
        string queue_type;
        string region;
        
        // General cham/summoner info
        int champ_id;
        public int part_index;
        bool is_winner;

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

        public PlayerRow(string match_version, long match_id, string region, string queue_type, RiotSharp.MatchEndpoint.Participant participant, RiotSharp.MatchEndpoint.ParticipantIdentity ident)
        {
            // Match info
            this.match_version = match_version;
            this.match_id = match_id;
            this.queue_type = queue_type;
            this.region = region;

            // Get participant and participant identity for this player
            RiotSharp.MatchEndpoint.ParticipantStats stat  = participant.Stats;

            // General cham/summoner info
            this.champ_id = participant.ChampionId;
            this.part_index = participant.ParticipantId;
            this.is_winner = stat.Winner;

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
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Champion: {0}", champ_id);
            sb.AppendLine();
            sb.AppendFormat("Did player win? {0}", is_winner);
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

            return sb.ToString();
        }

        public string get_sql()
        { 
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append("'" + this.match_version + "', ");
            sb.Append("'" + this.match_id + "', ");
            sb.Append("'" + this.queue_type + "', ");
            sb.Append("'" + this.region + "', ");
            sb.Append("'" + this.part_index + "', ");
            sb.Append("'" + this.champ_id + "', ");
            sb.Append("'" + this.is_winner + "', ");
            sb.Append("'" + this.damage_dealt + "', ");
            sb.Append("'" + this.damage_dealt_champs + "', ");
            sb.Append("'" + this.magic_damage_dealt + "', ");
            sb.Append("'" + this.magic_damage_dealt_champs + "', ");
            sb.Append("'" + this.healing_done + "', ");
            sb.Append("'" + this.minions_killed + "', ");
            sb.Append("'" + this.gold_earned + "', ");
            sb.Append("'" + this.kills + "', ");
            sb.Append("'" + this.deaths + "', ");
            sb.Append("'" + this.deaths + "'");
            sb.Append(")");

            return sb.ToString();
        }
    }
}
