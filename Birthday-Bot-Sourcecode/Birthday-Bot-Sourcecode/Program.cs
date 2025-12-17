using System.Threading;
using Common.Cfg;
using Discord;

namespace BotMaster
{
    public class BotFather
    {

        public static void Main(string[] args)
        {
            config cfg = configuration.data;
            var t = Task.Run(() => Discord.Discord.Main(cfg.DiscordToken, cfg.MasterDiscord));
            t.Wait();
        }
    }
}