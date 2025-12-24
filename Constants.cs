using System;
using System.Collections.Generic;
using System.Text;

namespace CribSheet
{
  public static class Constants
  {
    public const string DatabaseFilename = "CribSheet.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache |
        SQLite.SQLiteOpenFlags.FullMutex;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
  }
}

