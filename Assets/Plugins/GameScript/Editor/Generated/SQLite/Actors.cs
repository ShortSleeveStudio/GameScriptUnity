// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class Actors
    {
        public const string TABLE_NAME = "actors";
        public long id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public long localizedName { get; set; }
        public bool isSystemCreated { get; set; }

        public static Actors FromReader(SqliteDataReader reader)
        {
            Actors obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.name = reader.GetValue(1) is DBNull
                ? ""
                : reader.GetString(1)
                ;
            obj.color = reader.GetValue(2) is DBNull
                ? ""
                : reader.GetString(2)
                ;
            obj.localizedName = reader.GetValue(3) is DBNull
                ? 0
                : reader.GetInt64(3)
                ;
            obj.isSystemCreated = reader.GetValue(4) is DBNull
                ? false
                : reader.GetBoolean(4)
                ;
            return obj;
        }
    }
}
