// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class Locales
    {
        public const string TABLE_NAME = "locales";
        public long id { get; set; }
        public string name { get; set; }
        public bool isSystemCreated { get; set; }
        public long localizedName { get; set; }

        public static Locales FromReader(SqliteDataReader reader)
        {
            Locales obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.name = reader.GetValue(1) is DBNull
                ? ""
                : reader.GetString(1)
                ;
            obj.isSystemCreated = reader.GetValue(2) is DBNull
                ? false
                : reader.GetBoolean(2)
                ;
            obj.localizedName = reader.GetValue(3) is DBNull
                ? 0
                : reader.GetInt64(3)
                ;
            return obj;
        }
    }
}
