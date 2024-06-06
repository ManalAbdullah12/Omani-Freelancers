using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Project.Tables
{
    public class DatabaseHelper
    {
        readonly SQLiteConnection database;

        public DatabaseHelper(string dbPath)
        {
            database = new SQLiteConnection(dbPath);
            database.CreateTable<RegUserTable>();
        }

        public List<RegUserTable> GetAllUsers()
        {
            try
            {
                return database.Table<RegUserTable>().ToList();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during database operations
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }
}
