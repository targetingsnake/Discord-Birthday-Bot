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
        private TimeSpan loop_wait = new TimeSpan(0, 0, 30); //ToDo Set to 0, 5, 0 for production
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
                    if (!checkChannelExists(server, map[server]))
                    {
                        Console.WriteLine("Channel has been deleted.");
                        DatabaseConnector.instanze.setChannel(server, 0);
                        updateMap();
                    }
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

        private bool checkChannelExists(ulong serverId, ulong ChannelId)
        {
            SocketGuild discordServer = Discord.instanz.GetGuild(serverId);
            SocketChannel[] serverChannels = discordServer.Channels.ToArray();
            List<ulong> channelIds = new List<ulong>();
            foreach (SocketChannel channel in serverChannels)
            {
                channelIds.Add(channel.Id);
            }
            return channelIds.Contains(ChannelId);
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
                SocketGuild discordServer = Discord.instanz.GetGuild(server_id);
                SocketChannel[] serverChannels = discordServer.Channels.ToArray();
                List<ulong> channelIds = new List<ulong>();
                foreach (SocketChannel channel in serverChannels)
                {
                    channelIds.Add(channel.Id); 
                }
                ulong ChannelID = DatabaseConnector.instanze.getChannel(server_id);
                if (ChannelID != 0 && channelIds.Contains(ChannelID))
                {
                    mapping[server_id] = ChannelID;
                }
                else
                {
                    mapping[server_id] = discordServer.SystemChannel.Id;
                }
                    
            }
            Console.WriteLine("Posting Loop Map initialized");
            map = mapping;
        }

        public void reloadMap()
        {
            Console.WriteLine("Channel changed");
            updateMap();
        }
    }
}
