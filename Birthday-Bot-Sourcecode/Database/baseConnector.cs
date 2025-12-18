using Common.Cfg;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Con
{
    public class baseConnector
    {

        public baseConnector(config cfg) {
            string connectionString = $"Server={cfg.SQlServer};Port=3306;UserID={cfg.SQlUser};Password={cfg.SQlPassword};Database={cfg.SQLSchema};";
            // string connectionString = $"Server={cfg.SQlServer};Port=3306;UserID={cfg.SQlUser};Password={cfg.SQlPassword};Database={cfg.SQLSchema};SslMode=None;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            Console.WriteLine("Initalize Database:");
            MySqlCommand command = new MySqlCommand("show tables;", connection);
            MySqlDataReader reader = command.ExecuteReader();
            string result = "";
            while (reader.Read()){
                result += reader.GetString(0) + "\n";
            }
            Console.WriteLine(result);
            Console.WriteLine("Database Initialized.");
        }
    }
}
