using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace SqlLiteExample
{
    internal static class Program
    {
        public static String dbPath = "db.sqlite";

        public static void CreateDB()
        {
            SQLiteConnection.CreateFile(dbPath);
        }

        public static void CreateTable()
        {
            using (SQLiteConnection conn = new SQLiteConnection("URI=file:" + dbPath))
            {
                conn.Open();
                string sql = "Create Table Table1 (column1 varchar(20), column2 int)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
        }

        public static void Insert()
        {
            using (SQLiteConnection conn = new SQLiteConnection("URI=file:" + dbPath))
            {
                conn.Open();
                string sql = "Insert into Table1 (column1, column2) values (?, ?)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.Parameters.AddWithValue("@column1", "aaa");
                command.Parameters.AddWithValue("param2", 11);
                int updateCount = command.ExecuteNonQuery();
                Console.WriteLine(updateCount);
            }
        }

        public static void BatchInsert()
        {
            using (SQLiteConnection conn = new SQLiteConnection("URI=file:" + dbPath))
            {
                conn.Open();
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand command = new SQLiteCommand("Insert into Table1 (column1, column2) values (?, ?)", conn, transaction))
                        {
                            command.Parameters.AddWithValue("param1", "bbb");
                            command.Parameters.AddWithValue("param2", 22);
                            int updateCount = command.ExecuteNonQuery();
                            Console.WriteLine(updateCount);

                            command.Parameters.AddWithValue("param1", "ccc");
                            command.Parameters.AddWithValue("param2", 33);
                            updateCount = command.ExecuteNonQuery();
                            Console.WriteLine(updateCount);
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public static void Query()
        {
            using (SQLiteConnection conn = new SQLiteConnection("URI=file:" + dbPath))
            {
                conn.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM TABLE1", conn))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string c1 = reader.GetString("column1");
                            int c2 = reader.GetInt32("column2");
                            Console.WriteLine(c1 + ", " + c2);
                        }
                    }
                }
            }
        }

        public static void Main()
        {
            File.Delete(dbPath);
            CreateDB();
            CreateTable();
            Insert();
            BatchInsert();
            Query();
        }
    }
}