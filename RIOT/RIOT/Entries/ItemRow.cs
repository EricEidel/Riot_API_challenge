using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT
{
    public class ItemRow
    {
        long match_id;
        string queue_type;
        int participant;
        string region;

        public double time_ms;
        string event_type;
        int item_id;

        public ItemRow(RiotSharp.MatchEndpoint.Event frame_event, long match_id, RiotSharp.MatchEndpoint.QueueType queue_type, int participant, string region)
        {
            // Other info
            this.match_id = match_id;
            this.region = region;
            this.queue_type = queue_type.ToString();
            this.participant = participant;

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

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Participant ID {0} has {1} item {2} at {3} ms into the game.", this.participant, this.event_type, this.item_id, this.time_ms);

            return sb.ToString();
        }

        public string get_sql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append("'" + this.match_id + "', ");
            sb.Append("'" + this.queue_type + "', ");
            sb.Append("'" + this.region + "', ");
            sb.Append("'" + this.participant + "', ");
            sb.Append("'" + this.time_ms + "', ");
            sb.Append("'" + this.event_type + "', ");
            sb.Append("'" + this.item_id + "'");
            sb.Append(")");

            return sb.ToString();
        }
    }
}
