using Common.Cfg;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Database.Con
{
    public class baseConnector
    {

        private MySqlConnection connection = null;

        public baseConnector(config cfg) {
            string connectionString = $"Server={cfg.SQlServer};Port=3306;UserID={cfg.SQlUser};Password={cfg.SQlPassword};Database={cfg.SQLSchema};";
            connection = new MySqlConnection(connectionString);
            connection.Open();

            Console.WriteLine("Initalize Database:");
            MySqlCommand command = new MySqlCommand("show tables;", connection);
            MySqlDataReader reader = command.ExecuteReader();
            string result = "";
            while (reader.Read()){
                result += reader.GetString(0) + "\n";
            }
            reader.Close();
            Console.WriteLine(result);
            Console.WriteLine("Database Initialized.");
        }

        public void setBirthday(ulong guildID, ulong userid, int day, int month, int year)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO birthdays (uid, guid, day, month, year) VALUES ( @userid , @guildid , @day , @month , @year ) ON DUPLICATE KEY UPDATE uid = @userid , guid = @guildid , day = @day , month = @month , year = @year ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guildid", guildID);
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@day", day);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.ExecuteNonQuery();
        }

        public void setBirthday(ulong guildID, ulong userid, int day, int month)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO birthdays (uid, guid, day, month) VALUES ( @userid , @guildid , @day , @month ) ON DUPLICATE KEY UPDATE uid = @userid , guid = @guildid , day = @day , month = @month , year = NULL ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guildid", guildID);
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@day", day);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.ExecuteNonQuery();
        }

        public void deleteBirthday(ulong guildID, ulong userid)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM birthdays WHERE uid = @userid and guid = @guildid ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guildid", guildID);
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.ExecuteNonQuery();
        }

        public void setMod(ulong guildID, ulong roleid )
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO server (modrole, guid) VALUES ( @roleid , @guildid ) ON DUPLICATE KEY UPDATE modrole = @roleid ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guildid", guildID);
            cmd.Parameters.AddWithValue("@roleid", roleid);
            cmd.ExecuteNonQuery();
        }

        public void setChannel(ulong guildID, ulong chanelid)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO server (channelid, guid) VALUES ( @channelid , @guildid ) ON DUPLICATE KEY UPDATE channelid = @channelid ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guildid", guildID);
            cmd.Parameters.AddWithValue("@channelid", chanelid);
            cmd.ExecuteNonQuery();
        }

        public ulong getMod(ulong guildID)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "Select modrole from server where guid = @guildid ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guildid", guildID);
            MySqlDataReader reader = cmd.ExecuteReader();
            ulong result = 0;
            while (reader.Read())
            {
                result = reader.GetUInt64(0);
            }
            reader.Close();
            return result;
        }
        public ulong getChannel(ulong guildID)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "Select channelid from server where guid = @guildid ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guildid", guildID);
            MySqlDataReader reader = cmd.ExecuteReader();
            ulong result = 0;
            while (reader.Read())
            {
                result = reader.GetUInt64(0);
            }
            reader.Close();
            return result;
        }
    }
}
