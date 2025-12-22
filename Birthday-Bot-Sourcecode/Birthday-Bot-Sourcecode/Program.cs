using System.Threading;
using Common.Cfg;
using Discord;
using Database;
using System.Net;


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

            var t = Task.Run(() => Discord.Discord.Main(cfg));
            t.Wait();
        }
    }
}