using System.Threading;
using Common.Cfg;
using Discord;
using Database;
using System.Net;
using Watchdog;


namespace BotMaster
{
    public class BotFather
    {
        public static void Main(string[] args)
        {
            config cfg = configuration.data;
            DatabaseConnector.connect(cfg);


            WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
            Console.WriteLine(WebRequest.DefaultWebProxy);

            Thread watchdogThread = new Thread(Watchdog.Watchdog.Instance.watch);
            watchdogThread.Start();


            var t = Task.Run(() => Discord.Discord.Main(cfg));
            t.Wait();
        }
    }
}