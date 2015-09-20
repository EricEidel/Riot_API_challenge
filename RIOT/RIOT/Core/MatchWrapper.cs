using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT
{
    public class MatchWrapper
    {
        public Dictionary<int, PlayerRow> player_rows = new Dictionary<int, PlayerRow>();
        public Dictionary<int, List<ItemRow>> item_rows_per_player = new Dictionary<int, List<ItemRow>>();

        public long match_id;
        string queue_type;
        string region;
        string version;

        public MatchWrapper(RiotSharp.MatchEndpoint.MatchDetail match, string region)
        {
            this.match_id = match.MatchId;
            this.queue_type = match.QueueType.ToString();
            this.region = region;
            this.version = match.MatchVersion.Substring(0, 4);

            var participants = match.Participants;
            var idents = match.ParticipantIdentities;
            var timeline = match.Timeline;

            for (int i = 0; i < 10; i++)
            {
                PlayerRow r = new PlayerRow(version, this.match_id, this.region, this.queue_type, participants[i], idents[i]);
                player_rows.Add(i,r);
            }

            // Init item counters
            const int DEST = 0;
            const int BUY = 1;
            const int SELL = 2;
            const int UNDO = 4;
            Dictionary<int, Dictionary<int, int>> counters_by_part = new Dictionary<int, Dictionary<int, int>>();
            for (int i=0; i<10; i++)
            {   
                Dictionary<int, int> counters = new Dictionary<int, int>();
                counters.Add(DEST, 0);
                counters.Add(BUY, 0);
                counters.Add(SELL, 0);
                counters.Add(UNDO, 0);

                counters_by_part.Add(i, counters);
            }

            if (timeline == null)
            {
                throw new Exception("Timeline was not recieved for match ID: " + match.MatchId.ToString());
            }
            else if (timeline.Frames != null)
            {
                foreach (var frame in timeline.Frames)
                {
                    // There are no events at some frames - frame 0 for example.
                    if (frame.Events != null)
                    {
                        foreach (var frame_event in frame.Events)
                        {
                            int operation_index = -1;
                            // If it's one of the events we want to log
                            if (frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemDestroyed)
                                operation_index = DEST;
                            else if (frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemPurchased)
                                operation_index = BUY;
                            else if (frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemSold)
                                operation_index = SELL;
                            else if (frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemUndo)
                                operation_index = UNDO;
                            else
                                continue; // Not an item event
                            
                            if (frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemDestroyed && frame_event.ParticipantId == 0)
                                continue;

                            int item_id = frame_event.ItemId;
                            
                            int hp_pot = 2003;
                            int mana_pot = 2004;
                            int rej_bisc = 2009;
                            int rej_bisc_2 = 2010;
                            int ward = 2044;
                            int vision_ward = 2043;

                            // Don't proccess these
                            if (item_id == hp_pot || item_id == mana_pot || item_id == rej_bisc || item_id == rej_bisc_2 || item_id == ward || item_id == vision_ward)
                                continue;

                            // Get the dictionary list or create a new one
                            List<ItemRow> dictionary_list;
                            int index = frame_event.ParticipantId - 1;
                            var counter_by_operations = counters_by_part[index];
                            int current_num = counter_by_operations[operation_index];
                            counter_by_operations[operation_index]++;

                            if (item_rows_per_player.ContainsKey(index))
                            {
                                dictionary_list = item_rows_per_player[index];
                            }
                            else
                            {
                                dictionary_list = new List<ItemRow>();
                                item_rows_per_player.Add(index, dictionary_list);
                            }

                            // Generate the item row
                            var player_row = player_rows[index];
                            ItemRow item_row = new ItemRow(frame_event, player_row, current_num);

                            // Add the event to the dictionary
                            dictionary_list.Add(item_row);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Timeline frames were null for match ID: " + match.MatchId.ToString());
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Match ID: {0}, Queue Type: {1}, Region: {2}, Version: {3}", match_id, queue_type, region, version);
            sb.AppendLine();

            foreach (PlayerRow p_row in player_rows.Values )
            {
                sb.AppendLine(p_row.ToString());
                sb.AppendLine();

                sb.AppendLine("ITEM EVENTS:");
                var item_events = item_rows_per_player[p_row.part_index - 1];
                item_events.ForEach(x => sb.AppendLine(x.ToString()));
                sb.AppendLine();
                sb.AppendLine(" ------------------------------------------ ");
            }

            return sb.ToString();
        }
    }
}
