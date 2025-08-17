using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextHandle;

// этот файл содержит класс DBManager
namespace DB
{
    // класс для работы с базой данных
    public class WordStorage
    {
        private MySqlConnection? connection;
        public WordStorage()
        {
            try
            {
                string? connectionString;

                // получаем строку подключения из файла appsettings.json
                using (FileStream fs = new FileStream("C:\\Users\\Данил\\Desktop\\appsettings.json", FileMode.Open))
                {
                    JsonDocument doc = JsonDocument.Parse(fs);
                    connectionString = doc.RootElement.GetProperty("ConnectionStrings")
                                                 .GetProperty("MySqlDatabase")
                                                 .GetString();

                    if (connectionString == null)
                    {
                        throw new Exception("connection string is null");
                    }
                }

                this.connection = new MySqlConnection(connectionString);

                connection.Open();
            }
            catch (Exception ex)
            {
                connection.Close();
                Console.WriteLine("error when creating a connection to DB: " + ex.Message);
            }

            connection?.Close();
        }

        // возвращает список пар слово-сложность в указанной таблице tableName, необходимо передать существующие таблицы в tables для проверки tableName
        public WordDifficultyMap GetWords(string tableName)
        {
            List<WordDifficulty> result = new List<WordDifficulty>();
            List<string> tables = GetTableNames();
            try
            {
                if (connection == null) throw new Exception("connection to DB is null");

                if (!tables.Contains(tableName)) throw new Exception("no such table in existing tables");

                connection.Open();

                string sql = "SELECT Word, Difficulty FROM " + tableName + " ORDER BY Word ASC";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.Add(new WordDifficulty(rdr[0].ToString(), Double.Parse(rdr[1].ToString())));
                }

                rdr.Close();
                connection.Close();

                return new WordDifficultyMap(result);
            }
            catch (Exception ex)
            {
                connection.Close();
                Console.WriteLine("error when reading words from table \"" + tableName + "\": " + ex.Message);
            }

            return new WordDifficultyMap(result);
        }

        // возвращает список таблиц из базы данных
        public List<string> GetTableNames()
        {
            List<string> result = new List<string>();

            try
            {
                if (connection == null) throw new Exception("connection to DB is null");

                connection.Open();

                string sql = "SHOW TABLES";

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.Add(rdr[0].ToString());
                }

                rdr.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                Console.WriteLine("error when show tables: " + ex.Message);
            }

            return result;
        }

        // вставляет слова в указанную таблицу
        public void InsertWords(WordDifficultyMap wdm, string tableName)
        {
            try
            {
                if (connection == null) throw new Exception("connection to DB is null");

                if (!IsValidTableName(tableName)) throw new Exception("invalid table name");

                string sql;
                MySqlCommand cmd;

                if (!GetTableNames().Contains(tableName))
                {
                    connection.Open();

                    // создание таблицы
                    sql = "CREATE TABLE " + tableName + "(  Id INT PRIMARY KEY AUTO_INCREMENT, Word varchar(20), Difficulty DECIMAL(5, 3) )";
                    cmd = new MySqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();

                    // вставка элементов
                    sql = "INSERT INTO " + tableName + "(Word, Difficulty) VALUES (@word, @difficulty)";
                    cmd = new MySqlCommand(sql, connection);

                    cmd.Parameters.Add("@word", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@difficulty", MySqlDbType.Decimal);

                    foreach (WordDifficulty wd in wdm.GetMap())
                    {
                        cmd.Parameters["@word"].Value = wd.Word;
                        cmd.Parameters["@difficulty"].Value = wd.Difficulty;
                        cmd.ExecuteNonQuery();
                    }

                    connection.Close();
                }
                else
                {
                    // вставка элементов с учетом существующих
                    List<WordDifficulty> existed = GetWords(tableName).GetMap();

                    connection.Open();

                    sql = "INSERT INTO " + tableName + "(Word, Difficulty) VALUES (@word, @difficulty)";
                    cmd = new MySqlCommand(sql, connection);
                    cmd.Parameters.Add("@word", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@difficulty", MySqlDbType.Decimal);

                    foreach (WordDifficulty wd in wdm.GetMap())
                    {
                        if (existed.Contains(wd)) continue;

                        cmd.Parameters["@word"].Value = wd.Word;
                        cmd.Parameters["@difficulty"].Value = wd.Difficulty;
                        cmd.ExecuteNonQuery();
                        
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                Console.WriteLine("error when insert words to \"" + tableName + "\": " + ex.Message);
            }
        }

        public void DeleteWords(string tableName, params string[] words)
        {
            try
            {
                if (connection == null) throw new Exception("connection to DB is null");

                if (!IsValidTableName(tableName)) throw new Exception("invalid table name");

                if (!GetTableNames().Contains(tableName)) throw new Exception("no such table");

                string sql = "DELETE FROM " + tableName +
                    " WHERE Word = @word";

                connection.Open();

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.Add("@word", MySqlDbType.VarChar);

                foreach (string word in words)
                {
                    cmd.Parameters["@word"].Value = word;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error when delete table: " + ex.Message);
            }
        }

        public void DropTable(string tableName)
        {
            try
            {
                if (connection == null) throw new Exception("connection to DB is null");

                if (!IsValidTableName(tableName)) throw new Exception("invalid table name");

                if (!GetTableNames().Contains(tableName)) throw new Exception("no such table");

                string sql = "DROP TABLE " + tableName;

                connection.Open();

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();



                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error when delete table: " + ex.Message);
            }
        }

        private bool IsValidTableName(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]{0,63}$");
        }
    }
}
