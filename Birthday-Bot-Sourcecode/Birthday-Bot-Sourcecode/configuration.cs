using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Text.Json;
using Discord;
using System.Data;
using Common.Cfg;
using Discord.Rest;

namespace BotMaster
{
    internal class configuration
    {
        private static config readConfig()
        {
            string filename = "";
            if (File.Exists("overwrite.json"))
            {
                filename = "overwrite.json";
                Console.WriteLine("Overwrite found");
            }
            else if (File.Exists("dev.overwrite.json"))
            {
                filename = "dev.overwrite.json";
                Console.WriteLine("Deveopment overwrite found");
            }
            else if (File.Exists("config.json"))
            {
                filename = "config.json";
                Console.WriteLine("standard config found");
                throw new NotSupportedException("Usage of standard config not Supported, for safetyreasons use overwrite");
            }
            if (filename == "")
            {
                throw new FileNotFoundException("Configuration File not found");
            }
            string jsonString = File.ReadAllText(filename);
            InternalConfig cfg = JsonSerializer.Deserialize<InternalConfig>(jsonString)!;

            if (cfg.DiscordToken is null || cfg.SQlPassword is null
                || cfg.SQlServer is null || cfg.SQlUser is null || cfg.MasterDiscord is null || cfg.SQLSchema is null 
                || cfg.BirthdayWhishes is null || cfg.Debug is null || cfg.BirthdayWishesAge is null)
            {
                throw new DataException();
            }

            Console.WriteLine($"DB-Server: {cfg.SQlServer}");
            Console.WriteLine($"DB-Schema: {cfg.SQLSchema}");
            Console.WriteLine($"DB-User: {cfg.SQlUser}");
            Console.WriteLine($"DB-PW: *****************");
            Console.WriteLine($"DC-Token: *****************");
            string DcMaster = "";
            foreach (ulong t in cfg.MasterDiscord)
            {
                if (DcMaster != "")
                {
                    DcMaster += ", ";
                }
                DcMaster += t.ToString();
            }
            int _debug = cfg.Debug is null ? 0 : 1;
            string debugText = _debug == 1 ? "enabled" : "disabled";
            Console.WriteLine($"Debug-Mode: {debugText}");
            Console.WriteLine($"DC-Master: {DcMaster}");
            config Fcfg = new config(cfg.SQlPassword, cfg.SQlUser, cfg.SQlServer, cfg.SQLSchema, cfg.DiscordToken, cfg.MasterDiscord, cfg.BirthdayWhishes, cfg.BirthdayWishesAge, _debug);

            return Fcfg;
        }

        private static config _cfg = readConfig();

        public static config data
        {
            get
            {
                return _cfg;
            }
        }
    }

    public class InternalConfig
    {
        public string? SQlPassword { get; set; }
        public string? SQLSchema { get; set; }
        public string? SQlUser { get; set; }
        public string? SQlServer { get; set; }
        public string? DiscordToken { get; set; }
        public IList<ulong>? MasterDiscord { get; set; }
        public string[]? BirthdayWhishes { get; set; }
        public string[]? BirthdayWishesAge { get; set; }
        public int? Debug {  get; set; }   
    }
}
