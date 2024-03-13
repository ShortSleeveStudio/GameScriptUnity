// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class Version
    {
        public const string TABLE_NAME = "version";
        public long id { get; set; }
        public string version { get; set; }

        public static Version FromReader(SqliteDataReader reader)
        {
            Version obj = new();
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
