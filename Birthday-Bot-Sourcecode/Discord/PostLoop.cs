using Common;
using Common.Cfg;
using Database;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    internal class PostLoop
    {
        private ConcurrentDictionary<ulong, ulong> map = null;
        private TimeSpan loop_wait = new TimeSpan(0, 5, 0);
        private string[] birthdayWishes = null;

        private PostLoop()
        {

        }

        private static PostLoop _instance = new PostLoop();

        public static PostLoop Instance
        {
            get { return _instance; }
        }

        public void postLoop(object cfg)
        {
            updateMap();
            if (cfg.GetType() != typeof(config))
            {
                throw new Exception("Type missmatch");
            }
            birthdayWishes = ((config)cfg).BirthdayWhishes;
            while (true)
            {
                foreach (ulong server in map.Keys)
                {
                    postBirthdays(server, map[server]);
                }
                Thread.Sleep(loop_wait);
            }
        }

        private void postBirthdays(ulong serverId, ulong channelId)
        {
            Console.WriteLine($"Posting Birthdays for Server {Discord.instanz.GetGuild(serverId).Name}");
            List<Birthday> birthdayList = DatabaseConnector.instanze.getBirthdays(serverId);
            DateTime today = DateTime.Now;
            foreach (Birthday user in birthdayList){
                long lastPostedDB = user.lastPosted == -1 ? DateTime.MinValue.Ticks : user.lastPosted;
                DateTime lastPosted = new DateTime(lastPostedDB);
                if (!(lastPosted.Day == today.Day && lastPosted.Month == today.Month && lastPosted.Year == today.Year))
                {
                    Discord.instanz.GetGuild(serverId).GetTextChannel(channelId).SendMessageAsync(CreateEmbed(user.userID));
                    DatabaseConnector.instanze.setLastPosted(serverId, user.userID, today.Ticks);
                }
            }
        }

        private string CreateEmbed(ulong userID)
        {            
            Random random = new Random();
            string wishes = birthdayWishes[random.Next(0, birthdayWishes.Length)];
            wishes = wishes.Replace("%user%", $"<@{userID.ToString()}>");
            return wishes;
        }

        private void updateMap()
        {
            ConcurrentDictionary<ulong, ulong> mapping = new ConcurrentDictionary<ulong, ulong>();
            SocketGuild[] servers = Discord.instanz.Guilds.ToArray();
            List<ulong> server_ids = new List<ulong>();
            foreach (SocketGuild server in servers)
            {
                ulong id = server.Id;
                server_ids.Add(id);
            }
            foreach (ulong server_id in server_ids)
            {
                mapping[server_id] = DatabaseConnector.instanze.getChannel(server_id);
            }
            Console.WriteLine("Posting Loop Map initialized");
            map = mapping;
        }

        public void reloadMap()
        {
            updateMap();
        }
    }
}
