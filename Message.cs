using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDumper
{
    public readonly struct Message
    {
        public string content { get; init; }
        public string username { get; init; }
        public DateTime sent { get; init; }
        public string channelName { get; init; }
        public ulong userID { get; init; }
        public ulong messageID { get; init; }
        public ulong channelID { get; init; }
    }
}
