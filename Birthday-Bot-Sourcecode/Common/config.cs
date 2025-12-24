using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Cfg
{
    public class config
    {
        public config(string sQlPassword, string sQlUser, string sQlServer, string sQLSchema, string discordToken, IList<ulong> DcMaster, string[] _BirthdayWhishes, string[] _BirthdayWishesAge, int _debug)
        {
            SQlPassword = sQlPassword;
            SQlUser = sQlUser;
            SQlServer = sQlServer;
            SQLSchema = sQLSchema;
            DiscordToken = discordToken;
            ulong[] dcm = new ulong[DcMaster.Count];
            for (int j = 0; j < DcMaster.Count; j++)
            {
                dcm[j] = DcMaster[j];
            }
            MasterDiscord = dcm;
            BirthdayWhishes = _BirthdayWhishes;
            BirthdayWishesAge = _BirthdayWishesAge;
            debug = (_debug == 1);
        }

        public string SQlPassword { get; init; }
        public string SQLSchema { get; init; }
        public string SQlUser { get; init; }
        public string SQlServer { get; init; }
        public string DiscordToken { get; init; }
        public ulong[] MasterDiscord { get; init; }
        public string[] BirthdayWhishes { get; init; }
        public string[] BirthdayWishesAge { get; init; }
        public bool debug { get; init; }
    }

    
}
