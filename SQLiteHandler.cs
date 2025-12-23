using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace ProiectMIP
{
    internal class SQLiteHandler
    {
        private static SQLiteHandler instanta;
        private static readonly object blocare = new object();

        private SQLiteConnection conectare;

        private SQLiteHandler() { } //constructor privat sa nu pot face din exteriorul clasei SQLiteHandler 

        public static SQLiteHandler GetInstance()
        {
            if (instanta == null)
            {
                lock (blocare)//numai primul thread creeaza instanta
                {
                    if (instanta == null)
                    {
                        instanta = new SQLiteHandler();
                    }
                }
            }
            return instanta;
        }
        public void ConnectToDb(string path)
        {
            if (conectare != null)
                return;
            if (!File.Exists(path))//daca nu exista fisierul cu baza de date il creez
            {
                SQLiteConnection.CreateFile(path);
            }
            string Stringdeconectare = $"Data Source={path};Version=3;";

            conectare = new SQLiteConnection(Stringdeconectare);
            conectare.Open();
        }
        public void DisconnectFromDb()
        {
            if (conectare != null)
            {
                conectare.Close();
                conectare = null;
            }
        }
        public async Task<List<string>> GetAllKeywords()
        {
            List<string> keywords = new List<string>();

            string query = "SELECT keyword FROM Keywords ORDER BY keyword";

            using (var command = new SQLiteCommand(query, conectare))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        keywords.Add(reader.GetString(0));
                    }
                }
            }
            return keywords;
        }
        public void DeleteKeyword(string key)
        {
            string query = "DELETE FROM Keywords WHERE keyword = @key";
            using (var command = new SQLiteCommand(query, conectare))
            {
                command.Parameters.AddWithValue("@key", key);
                command.ExecuteNonQuery();
            }
        }
        public bool existakeyword(string keyword)
        {
            string query = "SELECT keyword FROM Keywords WHERE keyword = @key";
            using (var command = new SQLiteCommand(query, conectare))
            {
                command.Parameters.AddWithValue("@key", keyword); 
                using(var reader=command.ExecuteReader())
                {
                    return reader.Read(); //daca exista , inseamna ca este deja cuvantul cheie
                }
            }
        }
        public void AddKeyword(string keyword)
        {
            string query= "INSERT INTO Keywords (keyword) VALUES (@key)";

            using (var command = new SQLiteCommand(query, conectare))
            {
                command.Parameters.AddWithValue("@key", keyword);
                command.ExecuteNonQuery();
            }
        }
    }
}
