using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT
{
    public class MatchWrapper
    {
        public List<PlayerRow> player_rows = new List<PlayerRow>();
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
                player_rows.Add(r);
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
                            // If it's one of the events we want to log
                            if (frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemDestroyed ||
                                frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemPurchased ||
                                frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemSold ||
                                frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemUndo)
                            {

                                if (frame_event.EventType == RiotSharp.MatchEndpoint.EventType.ItemDestroyed && frame_event.ParticipantId == 0)
                                {
                                    //Console.WriteLine("Tower Item destroyed! Item ID: {0}, Participant ID: {1}", frame_event.ItemId, frame_event.ParticipantId);
                                    continue;
                                }                            

                                // Generate the item row
                                ItemRow item_row = new ItemRow(frame_event, match.MatchId, match.QueueType, frame_event.ParticipantId, region);

                                // Get the dictionary list or create a new one
                                List<ItemRow> dictionary_list;
                                int index = frame_event.ParticipantId - 1;

                                if (item_rows_per_player.ContainsKey(index))
                                {
                                    dictionary_list = item_rows_per_player[index];
                                }
                                else
                                {
                                    dictionary_list = new List<ItemRow>();
                                    item_rows_per_player.Add(index, dictionary_list);
                                }

                                // Add the event to the dictionary
                                dictionary_list.Add(item_row);
                            }
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

            foreach (PlayerRow p_row in player_rows)
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
