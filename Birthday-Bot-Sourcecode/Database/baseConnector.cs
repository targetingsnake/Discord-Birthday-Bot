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
        private string connectionString = "";

        public baseConnector(config cfg)
        {
            connectionString = $"Server={cfg.SQlServer};Port=3306;UserID={cfg.SQlUser};Password={cfg.SQlPassword};Database={cfg.SQLSchema};" +
                "Pooling=true;MinimumPoolSize=0;MaximumPoolSize=20;ConnectionIdleTimeout=30;ConnectionLifeTime=300;";
            Console.WriteLine("Initalize Databaseconnection");
        }

        public void setBirthday(ulong guildID, ulong userid, long day, long month, long year)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO birthdays (uid, guid, day, month, year) VALUES ( @userid , @guildid , @day , @month , @year )" +
                        " ON DUPLICATE KEY UPDATE uid = @userid , guid = @guildid , day = @day , month = @month , year = @year ;",
                        conn
                    ))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guildid", guildID);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void setBirthday(ulong guildID, ulong userid, long day, long month)
        {
            setBirthday(guildID, userid, day, month, -1);
        }

        public void deleteBirthday(ulong guildID, ulong userid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM birthdays WHERE uid = @userid and guid = @guildid ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guildid", guildID);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void setMod(ulong guildID, ulong roleid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO server (modrole, guid) VALUES ( @roleid , @guildid ) ON DUPLICATE KEY UPDATE modrole = @roleid ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guildid", guildID);
                    cmd.Parameters.AddWithValue("@roleid", roleid);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void setChannel(ulong guildID, ulong chanelid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO server (channelid, guid) VALUES ( @channelid , @guildid ) ON DUPLICATE KEY UPDATE channelid = @channelid ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guildid", guildID);
                    cmd.Parameters.AddWithValue("@channelid", chanelid);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void setTime(ulong guildID, long hour, long minute)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO server (posthour, postminute, guid) VALUES ( @posthour , @postminute , @guildid )" +
                    " ON DUPLICATE KEY UPDATE posthour = @posthour , postminute = @postminute ;",
                    conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guildid", guildID);
                    cmd.Parameters.AddWithValue("@posthour", hour);
                    cmd.Parameters.AddWithValue("@postminute", minute);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void addGreeting(ulong guildID, string text, int withAge)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO greetings (guid, text, with_age) VALUES ( @guid , @text , @with_age ) ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guid", guildID);
                    cmd.Parameters.AddWithValue("@text", text);
                    cmd.Parameters.AddWithValue("@with_age", withAge);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void deleteGreeting(ulong guildID, Int64 id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM greetings WHERE id = @id and guid = @guildid ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guildid", guildID);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public ulong getMod(ulong guildID)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("Select modrole from server where guid = @guildid ;", conn))
                {
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
            }
        }
        public ulong getChannel(ulong guildID)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("Select channelid from server where guid = @guildid ;", conn))
                {
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
            }
        }

        public postTime getPostTime(ulong guildID)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("Select posthour, postminute from server where guid = @guildid ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guildid", guildID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    postTime result = new postTime(0, 0);
                    try
                    {
                        while (reader.Read())
                        {
                            result = new postTime(reader.GetInt32("posthour"), reader.GetInt32("postminute"));
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
            }
        }

        public int[] getBirthday(ulong userId, ulong guildId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("Select day, month, year from birthdays where uid = @uid and guid = @guildid ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@guildid", guildId);
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
            }
        }

        public List<Birthday> getBirthdays(ulong serverID)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DateTime today = DateTime.Now;
                List<Birthday> users = new List<Birthday>();
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("Select uid, lastPosted, year from birthdays where guid = @guid and day = @day and month = @month  ;", conn))
                {
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
            }
        }

        public List<greeting> getGreetings(ulong serverID)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                List<greeting> greetings = new List<greeting>();
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("Select id, text, with_age from greetings where guid = @guid ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guid", serverID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            greetings.Add(new greeting(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
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
                    return greetings;
                }
            }
        }

        public void setLastPosted(ulong guildID, ulong userid, long timestamp)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE birthdays SET lastPosted = @lastPosted where guid = @guildid and uid = @userid ;", conn))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@guildid", guildID);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@lastPosted", timestamp);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
