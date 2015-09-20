using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT
{
    public class ItemRow
    {
        // Match info
        string match_version;
        long match_id;
        string queue_type;
        string region;
        long? summoner_id;
        string summoner_name = "";

        // General cham/summoner info
        int champ_id;
        public int part_index;
        bool is_winner;

        // Even id
        public double time_ms;
        string event_type;
        int item_id;

        int match_event_counter;

        public ItemRow(RiotSharp.MatchEndpoint.Event frame_event, PlayerRow player_row, int match_event_counter)
        {
            // Match info
            this.match_version = player_row.match_version;
            this.match_id = player_row.match_id;
            this.queue_type = player_row.queue_type;
            this.region = player_row.region;
            this.summoner_id = player_row.summoner_id;
            this.summoner_name = player_row.summoner_name;

            // General cham/summoner info
            this.champ_id = player_row.champ_id;
            this.part_index = player_row.part_index;
            this.is_winner = player_row.is_winner;

            // Even info
            this.time_ms = frame_event.Timestamp.Duration().TotalMilliseconds;
            this.item_id = frame_event.ItemId;

            // Parse the event type
            if ( frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemDestroyed)
            {
                event_type = "destroyed";
            }
            else if ( frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemPurchased)
            {
                event_type = "purchased";
            }
            else if ( frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemSold )
            {
                event_type = "sold";
            }
            else if ( frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemUndo )
            {
                event_type = "undone";
            }
            else
            {
                throw new Exception("[ItemRow] An event type of unexpected type was recieved:" + frame_event.EventType.ToString());
            }

            this.match_event_counter = match_event_counter;
        }

        public string get_sql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");

            // General match info
            sb.Append("'" + this.match_version + "', ");
            sb.Append("'" + this.match_id + "', ");
            sb.Append("'" + this.queue_type + "', ");
            sb.Append("'" + this.region + "', ");
            sb.Append("'" + this.part_index + "', ");
            sb.Append("'" + this.champ_id + "', ");
            sb.Append("'" + this.is_winner.ToString() + "', ");

            // Item event
            sb.Append("'" + this.time_ms + "', ");
            sb.Append("'" + this.event_type + "', ");
            sb.Append("'" + this.item_id + "', ");
            sb.Append("'" + this.match_event_counter + "' ");
            sb.Append(")");

            return sb.ToString();
        }
    }
}
