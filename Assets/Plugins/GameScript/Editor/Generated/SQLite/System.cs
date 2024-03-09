// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class System
    {
        public const string TABLE_NAME = "system";
        public long id { get; set; }
        public string version { get; set; }

        public static System FromReader(SqliteDataReader reader)
        {
            System obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.version = reader.GetValue(1) is DBNull
                ? ""
                : reader.GetString(1)
                ;
            return obj;
        }
    }
}
