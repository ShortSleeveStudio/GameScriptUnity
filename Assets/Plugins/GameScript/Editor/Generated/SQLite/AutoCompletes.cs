// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class AutoCompletes
    {
        public const string TABLE_NAME = "auto_completes";
        public long id { get; set; }
        public string name { get; set; }
        public long icon { get; set; }
        public long rule { get; set; }
        public string insertion { get; set; }
        public string documentation { get; set; }

        public static AutoCompletes FromReader(SqliteDataReader reader)
        {
            AutoCompletes obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.name = reader.GetValue(1) is DBNull
                ? ""
                : reader.GetString(1)
                ;
            obj.icon = reader.GetValue(2) is DBNull
                ? 0
                : reader.GetInt64(2)
                ;
            obj.rule = reader.GetValue(3) is DBNull
                ? 0
                : reader.GetInt64(3)
                ;
            obj.insertion = reader.GetValue(4) is DBNull
                ? ""
                : reader.GetString(4)
                ;
            obj.documentation = reader.GetValue(5) is DBNull
                ? ""
                : reader.GetString(5)
                ;
            return obj;
        }
    }
}
