using Common.Cfg;
using Common;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Database.Con
{
    public class baseConnector
    {

        private MySqlConnection connection = null;

        public baseConnector(config cfg)
        {
            string connectionString = $"Server={cfg.SQlServer};Port=3306;UserID={cfg.SQlUser};Password={cfg.SQlPassword};Database={cfg.SQLSchema};";
            connection = new MySqlConnection(connectionString);
            connection.Open();

            Console.WriteLine("Initalize Database:");
            MySqlCommand command = new MySqlCommand("show tables;", connection);
            MySqlDataReader reader = command.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                result += reader.GetString(0) + "\n";
            }
            reader.Close();
            Console.WriteLine(result);
            Console.WriteLine("Database Initialized.");
        }

        public void setBirthday(ulong guildID, ulong userid, long day, long month, long year)
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

        public void setBirthday(ulong guildID, ulong userid, long day, long month)
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

        public void setMod(ulong guildID, ulong roleid)
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
            try
            {
                while (reader.Read())
                {
                    result = reader.GetUInt64(0);
                }
            }
            catch
            {
                Console.WriteLine($"Mod-Role for Discord {guildID.ToString()} not set.");
            }
            finally
            {
                reader.Close();
            }
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
            try
            {
                while (reader.Read())
                {
                    result = reader.GetUInt64(0);
                }
            }
            catch
            {
                Console.WriteLine($"Channel for Discord {guildID.ToString()} not set.");
            }
            finally
            {
                reader.Close();
            }
            return result;
        }

        public int[] getBirthday(ulong userId)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "Select day, month, year from birthdays where uid = @uid ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@uid", userId);
            MySqlDataReader reader = cmd.ExecuteReader();
            int[] result = [0, 0, 0];
            try
            {
                while (reader.Read())
                {
                    result[0] = reader.GetInt32(0);
                    result[1] = reader.GetInt32(1);
                    object result3 = reader.GetValue(2);
                    if (result3 != null)
                    {
                        result[2] = (int)result3;
                    }
                }
            }
            catch
            {
                Console.WriteLine($"The Birthday of {userId.ToString()} has triggered an error.");
            }
            finally
            {
                reader.Close();
            }
            if (result[0] == 0 || result[1] == 0)
            {
                return null;
            }
            return result;
        }

        public List<Birthday> getBirthdays(ulong serverID)
        {
            DateTime today = DateTime.Now;
            List<Birthday> users = new List<Birthday>();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "Select uid, lastPosted, year from birthdays where guid = @guid and day = @day and month = @month  ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guid", serverID);
            cmd.Parameters.AddWithValue("@day", today.Day);
            cmd.Parameters.AddWithValue("@month", today.Month);
            MySqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                     users.Add(new Birthday(reader.GetUInt64(0), reader.GetInt64(1), reader.GetInt32(2)));
                }
            }
            catch
            {
                Console.WriteLine($"Something is wrong on {serverID.ToString()} and has triggered an error.");
            }
            finally
            {
                reader.Close();
            }
            return users;
        }

        public void setLastPosted(ulong guildID, ulong userid, long timestamp)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE birthdays SET lastPosted = @lastPosted where guid = @guildid and uid = @userid ;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@guildid", guildID);
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@lastPosted", timestamp);
            cmd.ExecuteNonQuery();
        }
    }
}
