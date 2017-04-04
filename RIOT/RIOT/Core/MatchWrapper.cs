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

        public long match_id;
        string queue_type;
        string region;
        string version;
        public DateTime timestamp;

        public MatchWrapper(RiotSharp.MatchEndpoint.MatchDetail match, string region)
        {
            this.timestamp = match.MatchCreation;
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
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Match ID: {0}, Queue Type: {1}, Region: {2}, Version: {3}, Creation time: {4}", match_id, queue_type, region, version, timestamp);
            
            sb.AppendLine();

            foreach (PlayerRow p_row in player_rows.Values)
            {
                sb.AppendLine(p_row.ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
